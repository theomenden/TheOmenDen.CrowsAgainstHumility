using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Services.Lobbies;

namespace TheOmenDen.CrowsAgainstHumility.Services.Hubs;
public sealed class CawChatHub: Hub
{
    public const string HubUrl = "/chat";
    private static readonly Dictionary<String, Player> ChatConnections = new(10);

    private readonly CawChatLobby _lobby;
    private readonly ILogger<CawChatHub> _logger;

    public CawChatHub()
    {
        
    }


}
