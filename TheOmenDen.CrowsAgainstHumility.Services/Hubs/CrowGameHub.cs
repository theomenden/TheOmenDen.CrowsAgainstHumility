using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Services.Lobbies;
using TheOmenDen.CrowsAgainstHumility.Services.Rooms;

namespace TheOmenDen.CrowsAgainstHumility.Services.Hubs;
public sealed class CrowGameHub: Hub
{
    public const string HubUrl = "/game";

    private static Dictionary<String, Player> ConnectionPlayers = new(10);

    private CrowGameLobby _lobby;
    private ILogger<CrowGameHub> _logger;

    public CrowGameHub(CrowGameLobby lobby, ILogger<CrowGameHub> logger)
    {
        _lobby = lobby;
        _logger = logger;
    }

    public List<RoomStateDto> GetRooms() => _lobby.GetRooms().ToList();

    public Task<bool> CreateRoom(String roomName, CrowRoomSettings roomSettings,
        CancellationToken cancellationToken = default)
    => _lobby.CreateRoom(roomName, roomSettings, cancellationToken);
}
