using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.DTO.Models.Players;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Settings;
using TheOmenDen.CrowsAgainstHumility.Core.Providers;
using TheOmenDen.CrowsAgainstHumility.Services.Decks;
using TheOmenDen.Shared.Guards;
using DateTimeProvider = TheOmenDen.CrowsAgainstHumility.Services.Providers.DateTimeProvider;
using TaskProvider = TheOmenDen.CrowsAgainstHumility.Services.Providers.TaskProvider;

namespace TheOmenDen.CrowsAgainstHumility.Services.Controllers;
internal sealed class CrowGameController
{
    private readonly ConcurrentDictionary<string, Tuple<Lobby, object>> _lobbies = new();
    private readonly DeckProcessor _deckProcessor;
    private readonly TaskProvider _taskProvider;
    private readonly ILogger<CrowGameController> _logger;

    public DateTimeProvider DateTimeProvider { get; private set; }
    public GuidProvider GuidProvider { get; private set; }
    public ICrowGameConfiguration Configuration { get; private set; }
    protected ICrowGameServerStore Repository { get; private set; }
    public IEnumerable<string> GetLobbyNames() => _lobbies.ToArray()
        .Select(x => x.Key)
        .Union(Repository.LobbyNames.ToArray(), StringComparer.OrdinalIgnoreCase)
        .ToArray() ?? Array.Empty<string>();


    private async Task<(Lobby loadedLobby, object lobbyLock)?> LoadLobbyAsync(string lobbyCode, CancellationToken cancellationToken = default)
    {
        var retry = true;
        ValueTuple<Lobby, object>? result;

        while (retry)
        {
            retry = false;

            if (!_lobbies.TryGetValue(lobbyName, out var lobbyTuple))
            {
                result = null;

                var lobbyToPull = await Repository.GetByLobbyCodeAsync(lobbyCode);

                if (lobbyToPull is null)
                {
                    continue;
                }

                if (VerifyLobbyIsActive(lobbyToPull))
                {
                    var lobbyLockObject = new object();
                    result = new(lobbyToPull, lobbyLockObject);

                    if (_lobbies.TryAdd(lobbyToPull.Name, result))
                    {
                        OnLobbyAdded(lobbyToPull);
                        continue;
                    }

                    result = null;
                    retry = true;
                    continue;
                }

                await Repository.RemoveAsync(lobbyToPull.Result, cancellationToken);
            }
        }

        return result;
    }
}
