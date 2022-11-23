using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Services.Hubs;
using TheOmenDen.CrowsAgainstHumility.Services.Rooms;

namespace TheOmenDen.CrowsAgainstHumility.Services.Lobbies;
public sealed class CrowGameLobby
{
    private readonly ILogger<CrowGameLobby> _logger;
    private IHubContext<CrowGameHub> _hubContext;
    private List<CrowGameRoom> _rooms = new(10);
    private Dictionary<Player, CrowGameRoom> _playersInRooms = new(10);
    private string _lobbyGroupName = Guid.NewGuid().ToString();

    public CrowGameLobby(IHubContext<CrowGameHub> hubContext, ILogger<CrowGameLobby> logger)
    {
        _hubContext= hubContext;
        _logger= logger;


    }

    internal void AddRoom(CrowGameRoom room)
    {
        var count = 0;

        lock (_rooms)
        {
            _rooms.Add(room);
            count = _rooms.Count;
        }

        _logger.LogInformation("Added new Room with Name {RoomName}. Total Active Rooms {Count}", room.RoomName, count);
    }

    internal IEnumerable<Player> GetPlayersInRoom(String roomName)
    {
        CrowGameRoom? room;
        
        lock (_rooms)
        {
            room = _rooms.FirstOrDefault(r => r.RoomName.Equals(roomName, StringComparison.OrdinalIgnoreCase));
        }

        return room?.Players ?? Enumerable.Empty<Player>();
    }

    internal IEnumerable<RoomStateDto> GetRooms()
    {
        lock (_rooms)
        {
            return _rooms.Select(r => r.ToRoomStateDto());
        }
    }

    internal Task AddPlayer(Player player) => _hubContext.Groups.AddToGroupAsync(player.ConnectionId, _lobbyGroupName);

    internal Task RemovePlayer(Player player) =>
        _hubContext.Groups.RemoveFromGroupAsync(player.ConnectionId, _lobbyGroupName);

    internal async Task PlayerDisconnected(Player player, CrowGameRoom room)
    {
        player.IsConnected = false;
        if (room.RoomState is CrowRoomStateLobby)
        {
            await LeaveRoom(player, room);
            return;
        }

        await _hubContext.Clients.GroupExcept(room.RoomName, player.ConnectionId)
            .SendAsync("PlayerConnectionStatusChanged", player.ToPlayerDto());
    }

    internal async Task PlayerReconnected(Player player, CrowGameRoom room)
    {
        await _hubContext.Clients.GroupExcept(room.RoomName, player.ConnectionId)
            .SendAsync("PlayerConnectionStatusChanged", player.ToPlayerDto());

        await _hubContext.Groups.AddToGroupAsync(player.ConnectionId, room.RoomName);

        await room.AddPlayer(player, true);
    }

    internal async Task<bool> CreateRoom(string roomName, CrowRoomSettings roomSettings,
        CancellationToken cancellationToken = default)
    {
        lock (_rooms)
        {
            if (String.IsNullOrWhiteSpace(roomName)
                || _rooms.Any(r => r.RoomName.Equals(roomName, StringComparison.InvariantCultureIgnoreCase)))
            {
                return false;
            }
        }

        var newRoom = new PublicRoom(_hubContext, roomName, roomSettings, GameEnded);
        AddRoom(newRoom);
        await _hubContext.Clients.Group(_lobbyGroupName).SendAsync("RoomCreated", newRoom.ToRoomStateDto(), cancellationToken: cancellationToken);
        return true;
    }

    internal async Task SetRoomSettings(CrowRoomSettings settings, CrowGameRoom room, Player player)
    {
        if (await room.SetRoomSettings(settings, player))
        {
            await _hubContext.Clients.Group(_lobbyGroupName).SendAsync("RoomStateChanged", room.ToRoomStateDto());
        }
    }

    internal CrowGameRoom? GetRoom(Player? player) =>
        player is not null 
        && _playersInRooms.TryGetValue(player, out var room) 
            ? room 
            : null;

    internal CrowGameRoom? GetRoom(String roomName)
    {
        lock (_rooms)
        {
            return _rooms.FirstOrDefault(r => roomName.Equals(r.RoomName, StringComparison.OrdinalIgnoreCase));
        }
    }

    internal async Task LeaveRoom(Player player, CrowGameRoom room)
    {
        if (_playersInRooms.Remove(player))
        {
            await room.RemovePlayer(player);

            if (!room.Players.Any())
            {
                lock (_rooms)
                {
                    _rooms.Remove(room);
                }

                await _hubContext.Clients.Group(_lobbyGroupName).SendAsync("RoomDeleted", room.ToRoomStateDto());
            }

            await _hubContext.Clients.GroupExcept(room.RoomName, player.ConnectionId)
                .SendAsync("PlayerLeft", player.ToPlayerDto());
            await _hubContext.Groups.RemoveFromGroupAsync(player.ConnectionId, room.RoomName);
            await _hubContext.Clients.Group(_lobbyGroupName).SendAsync("RoomStateChanged", room.ToRoomStateDto());
        }
    }

    internal async Task<bool> StartGame(CrowGameRoom room, Player player)
    {
        if (!room.StartGame(player))
        {
            return false;
        }

        await _hubContext.Clients.Group(_lobbyGroupName).SendAsync("RoomSateChanged", room.ToRoomStateDto());
        _logger.LogInformation("New Game started in room {Room} with {PlayerCount} players", room.RoomName, room.Players.Count());
        return true;

    }

    private async Task GameEnded(CrowGameRoom room, CancellationToken cancellationToken = default)
    {
        var disconnectedPlayers = Enumerable.Empty<Player>().ToList();
        
        disconnectedPlayers.AddRange(room.Players.Where(p => p.IsConnected));

        foreach (var player in disconnectedPlayers)
        {
            await LeaveRoom(player, room);
        }
    }
}
