using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Services.Hubs;

namespace TheOmenDen.CrowsAgainstHumility.Services.Lobbies;
public sealed class CawChatLobby
{
    private readonly ILogger<CawChatLobby> _logger;
    private readonly IHubContext<CawChatHub> _hubContext;
    private readonly string _lobbyGroupId = Guid.NewGuid().ToString();

    public CawChatLobby(IHubContext<CawChatHub> hubContext, ILogger<CawChatLobby> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }
}
