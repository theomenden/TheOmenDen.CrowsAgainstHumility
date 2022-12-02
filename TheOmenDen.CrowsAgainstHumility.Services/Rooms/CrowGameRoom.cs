using Microsoft.AspNetCore.SignalR;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Services.CardPoolBuilding;
using TheOmenDen.CrowsAgainstHumility.Services.Hubs;

namespace TheOmenDen.CrowsAgainstHumility.Services.Rooms;
public class CrowGameRoom
{
    private static int _roomCounter = 0;
    private readonly List<WhiteCard> _unusedWhiteCards = new(4000);
    private readonly List<BlackCard> _unusedBlackCards = new(50);
    private readonly List<BlackCard> _usedBlackCards = new(50);
    private List<WhiteCard> _usedWhiteCards = new(4000);
    private readonly List<Player> _players = new(10);
    private ICrowRoomState _roomState;

    public CrowGameRoom(IHubContext<CrowGameHub> context, String roomName, Func<CrowGameRoom, CancellationToken, Task> gameEndCallback)
    : this(context, roomName, new(), gameEndCallback) {}

    public CrowGameRoom(IHubContext<CrowGameHub> context, String roomName, CrowRoomSettings settings, Func<CrowGameRoom, CancellationToken, Task> gameEndCallback)
    {
        IncrementRoomCount();
        RoomIndex = _roomCounter;
        HubContext = context;
        RoomName = roomName;
        RoomSettings = settings;
        _roomState = new CrowRoomStateLobby(this, gameEndCallback);
        _ = _roomState.Enter();
    }

    internal Int32 RoomIndex { get; }

    internal IHubContext<CrowGameHub> HubContext { get; }

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
        List<PlayerDto> playersDto;

        lock (_players)
        {
            playersDto = _players.Select(p => p.ToPlayerDto()).ToList();
        }

        return new(RoomName, playersDto, RoomSettings, IsGameInProgress);
    }

    internal async Task PlayWhiteCard(Player player, WhiteCard whiteCard)
    {
        if (RoomState is RoomStateBlackCard rbc)
        {
            await rbc.SubmitCard(player, whiteCard);
            return;
        }

        var cm = new CrowChatMessage(CrowChatMessageType.Chat, "Played a card", player.Name);

        await SendAll("ChatMessage", cm);
    }

    internal void AddWhiteCardToPlayedWhiteCards(WhiteCard whiteCard)
    {
        _usedWhiteCards.Add(whiteCard);

        _unusedWhiteCards.Remove(whiteCard);
    }

    internal void AddRejectedWhiteCardsToPlayedWhiteCards(WhiteCard[] whiteCards)
    {
        _usedWhiteCards.AddRange(whiteCards);

        foreach (var whiteCard in whiteCards)
        {
            _unusedWhiteCards.Remove(whiteCard);
        }
    } 

    internal void MixRejectedWhiteCardsBackIntoPool()
    {
        var whiteCardsLeftInPool = _unusedWhiteCards.Count * 1.5;

        if (_usedWhiteCards.Count <= 0 || _unusedWhiteCards.Count <= whiteCardsLeftInPool)
        {
            return;
        }

        _unusedWhiteCards.AddRange(_usedWhiteCards);
        _usedWhiteCards.Clear();
    }

    internal void AddBlackCardToPlayedBlackCards(BlackCard blackCard)
    => _usedBlackCards.Add(blackCard);

    internal async Task Undo(Player player, CancellationToken cancellationToken = default)
    {
        if (_roomState is RoomStateBlackCard rsb)
        {
            await rsb.UndoSelection(player, cancellationToken);
        }
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
            isCardTsar = _players.FirstOrDefault(p => p.Id == player.Id)?.IsCardCzar ?? false;
        }

        if (!isCardTsar || RoomState is not CrowRoomStateLobby rsl)
        {
            return false;
        }

        RoomState = new RoomStateCrowGame(this, rsl);
        return true;

    }

    internal async Task<bool> SetRoomSettings(CrowRoomSettings settings, Player player) =>
        RoomState is CrowRoomStateLobby rsl 
        && await rsl.SetRoomSettings(settings, player);

    public Int32 RoundCount => RoomSettings.Rounds;

    public Boolean IsGameInProgress => _roomState is not CrowRoomStateLobby;

    #region SignalR SendAll Methods
    internal Task SendAll(string method, CancellationToken cancellationToken = default) => HubContext.Clients.Group(RoomName).SendAsync(method, cancellationToken: cancellationToken);
    internal Task SendAll(string method, object? arg, CancellationToken cancellationToken = default) => HubContext.Clients.Group(RoomName).SendAsync(method, arg, cancellationToken: cancellationToken);
    internal Task SendAll(string method, object? arg1, object? arg2, CancellationToken cancellationToken = default) => HubContext.Clients.Group(RoomName).SendAsync(method, arg1, arg2, cancellationToken: cancellationToken);
    internal Task SendAll(string method, object? arg1, object? arg2, object? arg3, CancellationToken cancellationToken = default) => HubContext.Clients.Group(RoomName).SendAsync(method, arg1, arg2, arg3, cancellationToken: cancellationToken);
    internal Task SendAll(string method, object? arg1, object? arg2, object? arg3, object? arg4, CancellationToken cancellationToken = default) => HubContext.Clients.Group(RoomName).SendAsync(method, arg1, arg2, arg3, arg4, cancellationToken: cancellationToken);
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
    internal Task SendAllExcept(Player player, string method, object? arg1, object? arg2, object? arg3, object? arg4, CancellationToken cancellationToken = default) =>
        HubContext.Clients.GroupExcept(RoomName, player.ConnectionId).SendAsync(method, arg1, arg2, arg3, arg4, cancellationToken: cancellationToken);
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

    private void GetNewBlackCard()
    {
        var randomizedProvider = new BlackCardProvider(_unusedBlackCards);

        _unusedBlackCards.Add(randomizedProvider.GetBlackCardForRound());

        _usedWhiteCards = new (10);
    }

    private static void IncrementRoomCount()
    {
        _roomCounter++;
    }
}
