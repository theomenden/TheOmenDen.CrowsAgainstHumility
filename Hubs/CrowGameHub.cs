using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using TheOmenDen.CrowsAgainstHumility.Bootstrapping;

namespace TheOmenDen.CrowsAgainstHumility.Hubs;


public class CrowGameHub : Hub<ICrowGame>
{
    public const string HubUrl = "/games";

    /// <summary>
    /// Stores the Game name as the key, and the usernames as the value - inspired by ApocDev (2022-10-12)
    /// </summary>
    public readonly ConcurrentDictionary<String, HashSet<String>> CurrentGames = new(StringComparer.OrdinalIgnoreCase);

    public async Task CreateGame(String username, String gameName, CancellationToken cancellationToken = default)
    {
        var currentId = Context.ConnectionId;

        if (CurrentGames.TryAdd(gameName, new HashSet<string> { username }))
        {
            await Groups.AddToGroupAsync(currentId, gameName, cancellationToken);

            await Clients.Group(gameName)
                .CreateGame(GameMessageTemplates.Created, $"{username} created a new game!", cancellationToken);
        }
    }

    public async Task JoinGame(String gameName, String username, CancellationToken cancellationToken = default)
    {
        var currentId = Context.ConnectionId;

        if (CurrentGames.TryGetValue(gameName, out var gamePopulation)
            && gamePopulation.Add(username))
        {
            await Groups.AddToGroupAsync(currentId, gameName, cancellationToken);
            
            await Clients.Group(gameName)
                .JoinGame(GameMessageTemplates.Joined, $"{username} joined the game!", cancellationToken);
        }
    }

    public async Task LeaveGame(String username, String gameName, CancellationToken cancellationToken = default)
    {
        var currentId = Context.ConnectionId;

        if (CurrentGames.TryGetValue(gameName, out var gamePopulation)
            && gamePopulation.Remove(username))
        {
            await Groups.RemoveFromGroupAsync(currentId, gameName, cancellationToken);

            await Clients.Group(gameName)
                .LeaveGame(GameMessageTemplates.Disconnected, $"{username} has left the group {gameName}.", cancellationToken);
        }
    }
}
