using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.Messages;

namespace TheOmenDen.CrowsAgainstHumility.Services.Hubs;
public sealed class CawChatHub: Hub
{
    private static readonly Dictionary<String, String> UserLookup = new (4);

    private readonly ILogger<CawChatHub> _logger;

    public CawChatHub(ILogger<CawChatHub> logger)
    {
        _logger = logger;   
    }

    public async Task SendMessage(String username, String message, CancellationToken cancellationToken = default)
        => await Clients.All.SendAsync(Messages.Received, username, message, cancellationToken);

    public async Task Register(String username, CancellationToken cancellationToken = default)
    {
        var currentId = Context.ConnectionId;

        if (!UserLookup.ContainsKey(username))
        {
            UserLookup.Add(currentId, username);

            await Clients.AllExcept(currentId)
                .SendAsync(Messages.Received, username, $"{username} joined the chat", cancellationToken);
        }
    }

    public override Task OnConnectedAsync()
    {
        _logger.LogInformation("Connected");
        return base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Disconnected {Ex}, {ContextId}", exception?.Message, Context.ConnectionId);

        var id = Context.ConnectionId;

        if (!UserLookup.TryGetValue(id, out var username))
        {
            username = "[unknown]";
        }

        UserLookup.Remove(id);

        await Clients.AllExcept(Context.ConnectionId)
            .SendAsync(Messages.Received, username, $"{username} has left the chat");

        await base.OnDisconnectedAsync(exception);
    }
}
