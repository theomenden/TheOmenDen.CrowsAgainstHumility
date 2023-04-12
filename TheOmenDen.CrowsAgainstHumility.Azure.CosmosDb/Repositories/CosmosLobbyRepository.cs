using System.Collections;
using System.Collections.Concurrent;
using System.Globalization;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Repositories;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Settings;
using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;
using TheOmenDen.CrowsAgainstHumility.Core.Models.Settings;
using TheOmenDen.CrowsAgainstHumility.Core.Providers;
using TheOmenDen.CrowsAgainstHumility.Core.Serialization;
using TheOmenDen.Shared.Extensions;

namespace TheOmenDen.CrowsAgainstHumility.Azure.CosmosDb.Repositories;
internal sealed class CosmosLobbyRepository : ICrowGameServerStore
{
    private const char SpecialCharacter = '%';
    private const string FileExtension = ".json";

    private readonly IDictionary<Guid, CrowGameServer> _servers;
    private readonly ICrowGameConfiguration _configuration;
    private readonly PlayerListSerializer _serializer;
    private readonly DateTimeProvider _dateTimeProvider;
    private readonly Lazy<char[]> _invalidCharacters;
    private readonly ILogger<CosmosLobbyRepository> _logger;
    private readonly Lazy<Container> _container;
    
    internal CosmosLobbyRepository(CosmosClient client)
    {
        _container = new(client.GetContainer("CrowsAgainstHumility", "CrowGames"));
        _servers = new ConcurrentDictionary<Guid, CrowGameServer>();
    }

    public CosmosLobbyRepository(
        IFilePlayerListRepositorySettings settings,
        ICrowGameConfiguration configuration,
        PlayerListSerializer serializer,
        DateTimeProvider dateTimeProvider,
        ILogger<CosmosLobbyRepository> logger,
        CosmosClient client)
    : this(client)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(logger);
        _servers = new ConcurrentDictionary<Guid, CrowGameServer>();
        _configuration = configuration ?? new CrowGameConfiguration();
        _serializer = serializer ?? new(dateTimeProvider, GuidProvider);
        _dateTimeProvider = dateTimeProvider ?? DateTimeProvider.Default;
        _invalidCharacters = new Lazy<char[]>(GetInvalidCharacters);
        _logger = logger;

        StringBuilderPoolFactory<CosmosLobbyRepository>.Create(nameof(CosmosLobbyRepository));
    }

    #region Public Properties
    public IEnumerable<string> LobbyNames { get; } = Enumerable.Empty<string>();

    public GuidProvider GuidProvider { get; } = GuidProvider.Default;
    #endregion
    public async Task<CrowGameServer> CreateAsync(Deck deck, CancellationToken cancellationToken = default)
    {
        CrowGameServer? server = null;
        try
        {
            var initializedContainer = _container.Value;

            var newServerId = GuidProvider.NewGuid();
            server = new CrowGameServer(newServerId, deck);
            _servers.Add(newServerId, server);
            await initializedContainer.CreateItemAsync(server, new PartitionKey(server.LobbyCode), cancellationToken: cancellationToken);

            return server;
        }

        catch (Exception e)
        {
            _logger.LogError("Could not create lobby with code {LobbyCode} due to exception {@Ex}", server?.LobbyCode, e);
            return server;
        }
    }

    public async Task<CrowGameServer> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _servers.TryGetValue(id, out var server);

        return server ?? throw new InvalidOperationException($"Server could not be found by Id: {id}");
    }

    public async Task<CrowGameServer> GetByLobbyCodeAsync(string lobbyCode, CancellationToken cancellationToken = default)
    {
        if (_servers.Any(s => s.Value.LobbyCode == lobbyCode))
        {
            return _servers.First(s => s.Value.LobbyCode == lobbyCode)
                .Value;
        }

        var initializedContainer = _container.Value;
        var containerResponse = await initializedContainer.ReadItemAsync<CrowGameServer>(
            id: lobbyCode,
            partitionKey: new PartitionKey(lobbyCode),
            cancellationToken: cancellationToken);

        return containerResponse.StatusCode is >= HttpStatusCode.OK and < HttpStatusCode.BadRequest
            ? containerResponse.Resource
            : throw new InvalidOperationException($"Could not retrieve server with lobby code {lobbyCode}");
    }

    public async Task<IEnumerable<CrowGameServer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        if (_servers.Any())
        {
            return _servers.Values.ToArray();
        }
        var initializedContainer = _container.Value;

        using var linqFeed = initializedContainer.GetItemQueryIterator<CrowGameServer>();

        // Convert to feed iterator

        var crowGames = Enumerable.Empty<CrowGameServer>().ToList();

        while (linqFeed.HasMoreResults)
        {
            var response = await linqFeed.ReadNextAsync(cancellationToken: cancellationToken);

            // Iterate query results
            crowGames.AddRange(response.Resource);
        }

        return crowGames;
    }

    public async IAsyncEnumerable<CrowGameServer?> GetAllAsyncStream([EnumeratorCancellation]CancellationToken cancellationToken = default)
    {
        if (_servers.Any())
        {
            foreach (var server in _servers)
            {
                yield return server.Value;
            }
        }
        var initializedContainer = _container.Value;

        using var linqFeed = initializedContainer.GetItemQueryStreamIterator(queryText: "SELECT * FROM CrowGames;");

        // Convert to feed iterator
        
        while (linqFeed.HasMoreResults)
        {
            var response = await linqFeed.ReadNextAsync(cancellationToken: cancellationToken);

            await foreach (var deserialized in JsonSerializer.DeserializeAsyncEnumerable<CrowGameServer>(
                               response.Content, JsonSerializerOptions.Default, cancellationToken))
            {
                yield return deserialized;
            }
        }
    }

    public async Task RemoveAsync(CrowGameServer server, CancellationToken cancellationToken = default)
    {
        if (!_servers.Remove(server.Id))
        {
            _logger.LogError("Server may have already been deleted with lobby code {LobbyCode}", server.LobbyCode);
        }

        var initializedContainer = _container.Value;

        var cosmosResponse = await initializedContainer.DeleteItemAsync<CrowGameServer>(server.LobbyCode, new PartitionKey(server.LobbyCode), cancellationToken: cancellationToken);


        if (cosmosResponse.StatusCode is >= HttpStatusCode.OK and < HttpStatusCode.BadRequest)
        {
            _logger.LogInformation("Server was removed for lobby {LobbyCode}", server.LobbyCode);
        }
    }

    #region Private Methods
    private string GetFileName(string lobbyName)
    {
        var result = StringBuilderPoolFactory<CosmosLobbyRepository>.Get(nameof(CosmosLobbyRepository)) ??
                     new StringBuilder();

        var invalidChars = _invalidCharacters.Value;

        foreach (var c in lobbyName)
        {
            var isSpecial = Array.BinarySearch(invalidChars, c, Comparer<char>.Default) >= 0;
            isSpecial = isSpecial || (c is not ' ' && Char.IsWhiteSpace(c));

            if (!isSpecial)
            {
                result.Append(c);
                continue;
            }

            result.Append(SpecialCharacter);
            result.Append(Convert.ToInt32(c).ToString("X4", CultureInfo.InvariantCulture));
        }

        return result.ToString();
    }
    #endregion
    #region Private Static Methods

    private static string GetLobbyName(string fileName)
    {
        var result = StringBuilderPoolFactory<CosmosLobbyRepository>.Get(nameof(CosmosLobbyRepository)) ?? new StringBuilder();
        return result.ToString();
    }
    private static char[] GetInvalidCharacters()
    {
        var invalidFileCharacters = Path.GetInvalidFileNameChars();
        var result = new char[invalidFileCharacters.Length + 1];
        result[0] = SpecialCharacter;
        invalidFileCharacters.CopyTo(result, 1);
        Array.Sort(result, Comparer<char>.Default);
        return result;
    }
    #endregion
    #region Interface Implementations
    public IEnumerator<CrowGameServer> GetEnumerator()
    {
        var initializedContainer = _container.Value;
        var serverEnumerator = initializedContainer
            .GetItemLinqQueryable<CrowGameServer>()
            .GetEnumerator();

        return serverEnumerator;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public async IAsyncEnumerator<CrowGameServer> GetAsyncEnumerator(CancellationToken cancellationToken = new CancellationToken())
    {
        var initializedContainer = _container.Value;
        
        using var linqFeed = initializedContainer.GetItemQueryStreamIterator(queryText: "SELECT * FROM CrowGames;");

        // Convert to feed iterator

        while (linqFeed.HasMoreResults)
        {
            var response = await linqFeed.ReadNextAsync(cancellationToken: cancellationToken);

            await foreach (var deserialized in JsonSerializer.DeserializeAsyncEnumerable<CrowGameServer>(
                               response.Content, JsonSerializerOptions.Default, cancellationToken))
            {
                yield return deserialized;
            }
        }
    }
    #endregion
}
