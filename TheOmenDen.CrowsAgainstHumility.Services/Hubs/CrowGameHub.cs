using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Services.Lobbies;
using TheOmenDen.CrowsAgainstHumility.Services.Rooms;

namespace TheOmenDen.CrowsAgainstHumility.Services.Hubs;
public sealed class CrowGameHub : Hub
{
    public const string HubUrl = "/games";
    private static readonly Dictionary<String, Player> ConnectionPlayers = new(10);

    private readonly CrowGameLobby _lobby;
    private readonly ILogger<CrowGameHub> _logger;

    public CrowGameHub(CrowGameLobby lobby, ILogger<CrowGameHub> logger)
    {
        _lobby = lobby;
        _logger = logger;
    }
    #region Methods that Concern Rooms
    public List<RoomStateDto> GetRooms() => _lobby.GetRooms().ToList();

    public Task<bool> CreateRoom(String roomName, CrowRoomSettings roomSettings,
        CancellationToken cancellationToken = default)
    => _lobby.CreateRoom(roomName, roomSettings, cancellationToken);

    public async Task<RoomStateDto?> JoinRoom(String roomName, String roomCode)
    {
        var player = GetPlayer(Context.ConnectionId);

        if (player is null)
        {
            return null;
        }

        var newRoom = _lobby.GetRoom(roomName);

        return newRoom is not null 
            ? await _lobby.JoinRoom(player, newRoom, roomCode) 
            : null;
    }

    public async Task<Boolean> LeaveRoom()
    {
        var player = GetPlayer(Context.ConnectionId);

        var room = _lobby.GetRoom(player);

        if (player is null || room is null)
        {
            return false;
        }

        await _lobby.LeaveRoom(player, room);
        await _lobby.AddPlayer(player);

        return true;
    }
    #endregion
    #region Connection Methods
    public override async Task OnConnectedAsync()
    {
        var transportType = Context.Features.Get<IHttpTransportFeature>()?.TransportType;

        _logger.LogDebug("OnConnected SignalR TransportType: {TransportType}", transportType);

        Player? player = null;

        lock (ConnectionPlayers)
        {
            if (!ConnectionPlayers.TryGetValue(Context.ConnectionId, out player))
            {
                player = new Player(Context.ConnectionId);
                ConnectionPlayers.Add(Context.ConnectionId, player);
            }
        }

        if (player is not null)
        {
            await _lobby.AddPlayer(player);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (exception is not null)
        {
            _logger.LogWarning("Player disconnected with exception {@Ex}", exception);
        }

        var player = GetPlayer(Context.ConnectionId);

        if (player is null)
        {
            await base.OnDisconnectedAsync(exception);
            return;
        }

        var room = _lobby.GetRoom(player);

        if (room is not null)
        {
            await _lobby.PlayerDisconnected(player, room);
            await base.OnDisconnectedAsync(exception);
            return;
        }

        await _lobby.RemovePlayer(player);
        lock (ConnectionPlayers)
        {
            ConnectionPlayers.Remove(Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task<ReconnectionStateDto> TryToReconnect(String username, Guid connectionId)
    {
        Player? existingPlayer;
        lock (ConnectionPlayers)
        {
            existingPlayer = ConnectionPlayers.Values.SingleOrDefault(player => player.ConnectionGuid.Equals(connectionId)
            || player.Username == username);
        }

        if (existingPlayer is null || existingPlayer.IsConnected)
        {
            return new ReconnectionStateDto(Guid.Empty, null);
        }

        lock (ConnectionPlayers)
        {
            ConnectionPlayers.Remove(Context.ConnectionId);
            ConnectionPlayers.Remove(existingPlayer.ConnectionId);

            existingPlayer.ConnectionId = Context.ConnectionId;
            existingPlayer.IsConnected = true;

            ConnectionPlayers.Add(Context.ConnectionId, existingPlayer);
        }

        var room = _lobby.GetRoom(existingPlayer);

        if (room is null || room.RoomState is CrowRoomStateLobby)
        {
            return new ReconnectionStateDto(Guid.Empty, null);
        }

        await _lobby.PlayerReconnected(existingPlayer, room);
        return new ReconnectionStateDto(existingPlayer.Id, room.ToRoomStateDto());
    }
    #endregion
    #region Methods that concern game states
    public async Task<bool> StartGame()
    {
        var player = GetPlayer(Context.ConnectionId);
        var room = _lobby.GetRoom(player);

        if (room is not null && player is not null)
        {
           return await _lobby.StartGame(room, player);
        }

        return false;
    }

    public void ChooseWhiteCard(int cardIndex)
    {
        var player = GetPlayer(Context.ConnectionId);
        var room = _lobby.GetRoom(player);

        if (room is not null && player is not null)
        {
           // room.ChooseWhiteCard(cardIndex, player);
        }
    }

    public async Task PlayWhiteCard(WhiteCard card)
    {
        var player = GetPlayer(Context.ConnectionId);
        var room = _lobby.GetRoom(player);

        if (room is not null && player is not null)
        {
            room.AddWhiteCardToPlayedWhiteCards(card);
        }
    }
    #endregion
    
    private static Player? GetPlayer(String connectionId)
    {
        lock (ConnectionPlayers)
        {
            if (ConnectionPlayers.TryGetValue(connectionId, out var player))
            {
                return player;
            }
        }

        return null;
    }
}
