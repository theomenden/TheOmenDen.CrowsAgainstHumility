using Microsoft.AspNetCore.SignalR;

namespace TheOmenDen.CrowsAgainstHumility.Hubs;
internal sealed class CawHub : Hub
{
    public const string HubUrl = "/chat";

    public async Task Broadcast(string username, string message) =>
        await Clients.All.SendAsync("Broadcast", username, message);

    public Task JoinRoom(String roomId)
    {
        return Groups.AddToGroupAsync(Context.ConnectionId, roomId);
    }

    public Task LeaveRoom(String roomId)
    {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
    }

    public override Task OnConnectedAsync()
    {
        Console.WriteLine($"{Context.ConnectionId} connected");
        return base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"Disconnected {exception?.Message} {Context.ConnectionId}");
        await base.OnDisconnectedAsync(exception);
    }
}
