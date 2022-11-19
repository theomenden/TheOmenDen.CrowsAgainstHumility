using Microsoft.AspNetCore.SignalR;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Services.Hubs;

namespace TheOmenDen.CrowsAgainstHumility.Services.Rooms;
public class CrowGameRoom
{
    private static int RoomCounter = 0;

    private int _roomIndex;
    private List<BlackCard> _unusedBlackCards = new(50);
    private List<WhiteCard> _unusedWhiteCards = new(4000);
    private List<Player> _players = new(10);
    private readonly IHubContext<CrowGameHub> _hubContext;
    private ICrowRoomState _roomState;

    public CrowGameRoom(IHubContext<CrowGameHub> context, String roomName, Func<CrowGameRoom, CancellationToken, Task> gameEndCallback)
    : this(context, roomName, new(), gameEndCallback) {}

    public CrowGameRoom(IHubContext<CrowGameHub> context, String roomName, CrowRoomSettings settings, Func<CrowGameRoom, CancellationToken, Task> gameEndCallback)
    {
        _roomIndex = RoomCounter++;
        _hubContext = context;
        RoomName = roomName;
        RoomSettings = settings;
        _roomState = new CrowRoomStateLobby(this, gameEndCallback);
        _ = _roomState.Enter();
    }

    internal Int32 RoomIndex => _roomIndex;
    internal IHubContext<CrowGameHub> HubContext => _hubContext;

    public IEnumerable<Player> Players
    {
        get
        {
            lock (_players)
            {
                return _players.ToArray();
            }
        }
    }

    public String RoomName { get; }

    public CrowRoomSettings RoomSettings { get; internal set; }

    internal ICrowRoomState RoomState
    {
        get => _roomState;
        set
        {
            if (value is null || _roomState == value)
            {
                return;
            }

            _roomState = value;
            _ = _roomState.Enter();
        }
    }

    internal RoomStateDto ToRoomStateDto()
    {
        var playersDto = Enumerable.Empty<PlayerDto>().ToList();

        lock (_players)
        {
            playersDto = _players.Select(p => p.ToPlayerDto()).ToList();
        }

        return new(RoomName, playersDto, RoomSettings, IsGameInProgress);
    }

    public async Task AddPlayer(Player player, bool isReconnection = false, CancellationToken cancellationToken = default)
    {
        if (!isReconnection)
        {
            lock (_players)
            {
                _players.Add(player);
            }
        }

        await _roomState.AddCrow(player, isReconnection, cancellationToken);
    }

    public async Task<bool> RemovePlayer(Player player, CancellationToken cancellationToken = default)
    {
        bool wasPlayerRemoved;
        lock (_players)
        {
            wasPlayerRemoved= _players.Remove(player);
        }

        if (!wasPlayerRemoved)
        {
            return false;
        }

        await _roomState.RemoveCrow(player, cancellationToken);
        return true;
    }

    internal bool StartGame(Player player)
    {
        bool isCardTsar;
        lock (_players)
        {
            isCardTsar = _players.FirstOrDefault(p => p.Id == player.Id && p.IsCardTsar);
        }

        if (isCardTsar && RoomState is CrowRoomStateLobby rsl)
        {
            RoomState = new RoomStateCrowGame(this, rsl);
            return true;
        }

        return false;
    }

    public Int32 RoundCount => RoomSettings.Rounds;

    public Boolean IsGameInProgress => _roomState is not CrowRoomStateLobby;

    #region SignalR SendAll Methods
    internal Task SendAll(string method, CancellationToken cancellationToken = default) => HubContext.Clients.Group(RoomName).SendAsync(method, cancellationToken: cancellationToken);
    internal Task SendAll(string method, object? arg, CancellationToken cancellationToken = default) => HubContext.Clients.Group(RoomName).SendAsync(method, arg, cancellationToken: cancellationToken);
    internal Task SendAll(string method, object? arg1, object? arg2, CancellationToken cancellationToken = default) => HubContext.Clients.Group(RoomName).SendAsync(method, arg1, arg2, cancellationToken: cancellationToken);
    internal Task SendAll(string method, object? arg1, object? arg2, object? arg3, CancellationToken cancellationToken = default) => HubContext.Clients.Group(RoomName).SendAsync(method, arg1, arg2, arg3, cancellationToken: cancellationToken);
    #endregion
    #region SignalR SendAllExceptMethods
    internal Task SendAllExcept(Player player, string method, CancellationToken cancellationToken = default) =>
        HubContext.Clients.GroupExcept(RoomName, player.ConnectionId).SendAsync(method, cancellationToken: cancellationToken);
    internal Task SendAllExcept(Player player, string method, object? arg, CancellationToken cancellationToken = default) =>
        HubContext.Clients.GroupExcept(RoomName, player.ConnectionId).SendAsync(method, arg, cancellationToken: cancellationToken);
    internal Task SendAllExcept(Player player, string method, object? arg1, object? arg2, CancellationToken cancellationToken = default) =>
        HubContext.Clients.GroupExcept(RoomName, player.ConnectionId).SendAsync(method, arg1, arg2, cancellationToken: cancellationToken);
    internal Task SendAllExcept(Player player, string method, object? arg1, object? arg2, object? arg3, CancellationToken cancellationToken = default) =>
        HubContext.Clients.GroupExcept(RoomName, player.ConnectionId).SendAsync(method, arg1, arg2, arg3, cancellationToken: cancellationToken);
    #endregion
    #region SendPlayer Methods
    internal Task SendPlayer(Player player, string method, CancellationToken cancellationToken = default) => HubContext.Clients.Client(player.ConnectionId).SendAsync(method, cancellationToken: cancellationToken);
    internal Task SendPlayer(Player player, string method, object? arg, CancellationToken cancellationToken = default) => HubContext.Clients.Client(player.ConnectionId).SendAsync(method, arg, cancellationToken: cancellationToken);
    internal Task SendPlayer(Player player, string method, object? arg1, object? arg2, CancellationToken cancellationToken = default)
        => HubContext.Clients.Client(player.ConnectionId).SendAsync(method, arg1, arg2, cancellationToken: cancellationToken);
    internal Task SendPlayer(Player player, string method, object? arg1, object? arg2, object? arg3, CancellationToken cancellationToken = default)
        => HubContext.Clients.Client(player.ConnectionId).SendAsync(method, arg1, arg2, arg3, cancellationToken: cancellationToken);
    internal Task SendPlayer(Player player, string method, object? arg1, object? arg2, object? arg3, object? arg4, CancellationToken cancellationToken = default)
        => HubContext.Clients.Client(player.ConnectionId).SendAsync(method, arg1, arg2, arg3, arg4, cancellationToken: cancellationToken);
    #endregion
}
