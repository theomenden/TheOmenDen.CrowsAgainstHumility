using Microsoft.AspNetCore.SignalR;

namespace TheOmenDen.CrowsAgainstHumility.Hubs;

public class CrowGameHub : Hub
{
    public const string HubUrl = "/games";

    private static readonly Dictionary<string, HashSet<string>> Games= new (100, StringComparer.OrdinalIgnoreCase);

    public Task CreateGame(String gameName)
    {
        var currentConnection = Context.ConnectionId;

        var players = new HashSet<String> { currentConnection };

        Games.Add(gameName, players);

        Groups.AddToGroupAsync(gameName, currentConnection);

        return Clients.Group(gameName).SendAsync("Send", $"{currentConnection} has created the game {gameName}");
    }

    public Task JoinGame(String gameName)
    {
        var currentConnection = Context.ConnectionId;

        if (!Games.TryGetValue(gameName, out var players))
        {
            Games.Add(gameName, new HashSet<string>{ currentConnection });
        }
        
        players!.Add(currentConnection);

        return Groups.AddToGroupAsync(currentConnection, gameName);
    }

    public Task LeaveGame(String gameName)
    {
        var currentConnection = Context.ConnectionId;

        if (!Games.TryGetValue(gameName, out var players))
        {
            players?.Remove(currentConnection);
        }

        return Groups.RemoveFromGroupAsync(currentConnection, gameName);
    }
}
