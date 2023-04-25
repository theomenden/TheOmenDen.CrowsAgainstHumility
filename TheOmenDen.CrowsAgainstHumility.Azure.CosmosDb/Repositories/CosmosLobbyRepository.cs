using System.Collections.Concurrent;
using System.Net;
using System.Runtime.CompilerServices;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Azure.CosmosDb.Extensions;
using TheOmenDen.CrowsAgainstHumility.Core.DTO.ViewModels;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Game;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Models;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Stores;
using TheOmenDen.CrowsAgainstHumility.Core.Models.Settings;
using TheOmenDen.CrowsAgainstHumility.Core.Providers;

namespace TheOmenDen.CrowsAgainstHumility.Azure.CosmosDb.Repositories;
internal sealed class CosmosLobbyRepository: IServerStore
{
    private readonly IDictionary<Guid, CrowGameServer> _servers;
    private readonly IGuidProvider _guidProvider;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<CosmosLobbyRepository> _logger;
    private readonly Container _container;

    public CosmosLobbyRepository(CosmosClient client,
        CosmosDbSettings settings,
        ILogger<CosmosLobbyRepository> logger,
        IGuidProvider? guidProvider,
        IDateTimeProvider? dateTimeProvider)
    {
        _logger = logger;
        _container = client.GetContainer(settings.DatabaseName, settings.ContainerName) ?? throw new ArgumentException(nameof(_container));
        _servers = new ConcurrentDictionary<Guid, CrowGameServer>();
        _guidProvider = guidProvider ?? GuidProvider.Default;
        _dateTimeProvider = dateTimeProvider ?? DateTimeProvider.Default;
    }

    public async Task<CrowGameServer> CreateServerAsync(CreateCrowGameViewModel configuration, CancellationToken cancellationToken = default)
    {

        var newServerId = _guidProvider.NewGuid();
        
        var newServer = new CrowGameServer(newServerId,configuration.Name,configuration.Code, new ConcurrentDictionary<string, Player>(), new CrowGameSession(configuration.DesiredCards), _dateTimeProvider.UtcNow);
        _servers.Add(newServerId, newServer);
        try
        {
           var cosmosResponse = await _container.CreateItemAsync(newServer, new PartitionKey(newServer.Code), cancellationToken: cancellationToken);

           if (cosmosResponse.StatusCode >= HttpStatusCode.BadRequest)
           {
               throw new InvalidOperationException();
           }
        }
        catch (Exception ex)
        {
            _logger.LogError("Unable to add server '{ServerName}' due to : {Ex}", newServer.Name, ex.Message);
            throw;
        }

        return newServer;
    }

    public async Task<bool> RemoveServerAsync(CrowGameServer serverToRemove, CancellationToken cancellationToken = default)
    {
        if (!_servers.Remove(serverToRemove.Id))
        {
            return false; //At this point, the server should be removed from both Cosmos and our local store.
        }

        try
        {
            var cosmosResponse = await _container.DeleteItemAsync<CrowGameServer>(serverToRemove.Code, new PartitionKey(), cancellationToken: cancellationToken);

            if (cosmosResponse.StatusCode >= HttpStatusCode.BadRequest)
            {
                throw new InvalidOperationException();
            }
        }
        catch (Exception e)
        {
            _logger.LogError("Could not remove the server '{Server}' from the Cosmos Repo, due to {Exception}", serverToRemove.Code, e.Message);
            return false;
        }

        return true;
    }

    public async IAsyncEnumerable<bool> RemoveAllServersAsyncStream([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        _servers.Clear();

        var query = _container.GetItemLinqQueryable<CrowGameServer>();

        var feedIterator = query
            .Where(s => s.CurrentSession.CanPlay)
            .ToFeedIterator();

        await foreach (var server in feedIterator
                           .ToAsyncEnumerable()
                           .WithCancellation(cancellationToken))
        {
            bool isDeleted;
            try
            {
                await _container.DeleteItemAsync<CrowGameServer>(server.Code, PartitionKey.None,
                    cancellationToken: cancellationToken);
                isDeleted = true;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Server could have already been removed from Cosmos: {@Ex}", ex);
                isDeleted = false;
            }

            yield return isDeleted;
        }
    }

    public async Task<CrowGameServer> GetServerByIdAsync(Guid serverId, CancellationToken cancellationToken = default)
    {
        if (_servers.TryGetValue(serverId, out var result))
        {
            return result;
        }

        try
        {
            var cosmosResponse = await _container.ReadItemAsync<CrowGameServer>(serverId.ToString(), new PartitionKey(),
                cancellationToken: cancellationToken);

            result = cosmosResponse.Resource;

            _servers.TryAdd(result.Id, result);
        }
        catch (Exception ex)
        {
            _logger.LogError("Could not find lobby with Id '{LobbyId}", serverId);
            throw;
        }

        return result;
    }

    public async Task<CrowGameServer> GetServerByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var server = _servers.Values.FirstOrDefault(s => String.Equals(s.Code, code, StringComparison.OrdinalIgnoreCase));
        if (server is not null)
        {
            _logger.LoadLobby(server.Id.ToString());
            return server;
        }

        try
        {
            var cosmosResponse =
                await _container.ReadItemAsync<CrowGameServer>(code, new PartitionKey(),
                    cancellationToken: cancellationToken);

            server = cosmosResponse.Resource;

            return server;
        }
        catch(Exception ex) 
        {
            _logger.LogError("Could not find lobby with code: '{Code}'", code);
            throw;
        }

    }

    public async Task<int> GetTotalSessionsAsync(CancellationToken cancellationToken = default)
    {
        var totalServers = _servers.Count;

        try
        {
            var cosmosQuery = _container.GetItemLinqQueryable<CrowGameServer>();

            var count = (await cosmosQuery.CountAsync(cancellationToken)).Resource;

            totalServers = totalServers > count ? totalServers : count;
        }
        catch
        {
            totalServers = 0;
        }

        return totalServers;
    }

    public Task<int> GetTotalPlayersAsync(CancellationToken cancellationToken = default)
    {
        var totalPlayers = _servers.Values.Sum(s => s.Players.Count);

        try
        {
            var cosmosQuery = _container.GetItemLinqQueryable<CrowGameServer>();
            
            var count = cosmosQuery.Sum(p => p.Players.Count);

            totalPlayers = totalPlayers > count ? totalPlayers : count;
        }
        catch
        {
            totalPlayers = 0;
        }

        return Task.FromResult(totalPlayers);
    }

    public async IAsyncEnumerable<CrowGameServer> GetAllServersAsyncStream([EnumeratorCancellation]CancellationToken cancellationToken = default)
    {
        var query = _container.GetItemLinqQueryable<CrowGameServer>();

        var feedIterator = query.Where(s => s.CurrentSession.CanPlay)
            .ToFeedIterator();

        await foreach (var server in feedIterator
                           .ToAsyncEnumerable()
                           .WithCancellation(cancellationToken))
        {
            yield return server;
        }
    }
}
