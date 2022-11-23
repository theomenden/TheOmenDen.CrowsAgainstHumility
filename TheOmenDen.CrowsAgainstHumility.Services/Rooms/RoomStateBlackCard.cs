using System.Timers;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Services.Helpers;

namespace TheOmenDen.CrowsAgainstHumility.Services.Rooms;
internal sealed class RoomStateBlackCard: ICrowRoomState
{
    private static readonly object Locking = new ();

    private readonly Player _cardTsar;
    private readonly CrowGameRoom _room;
    private readonly WhiteCard _chosenWhiteCard;
    private readonly RoomStateCardTsarTurn _roomStateCardTsarTurn;

    private bool _turnEnded = false;
    private int _turnTime;
    private readonly CrowGameTimer _timer;
    private List<WhiteCard> _whiteCardsPlayed = Enumerable.Empty<WhiteCard>().ToList();
    private readonly List<Player> _playersSubmitting;
    private readonly List<ValueTuple<Player, WhiteCard>> _playerResults = Enumerable.Empty<ValueTuple<Player, WhiteCard>>().ToList();
    private readonly List<CrowChatMessage> _chatLog = Enumerable.Empty<CrowChatMessage>().ToList();

    public RoomStateBlackCard(Player player, CrowGameRoom room, WhiteCard chosenWhiteCard, RoomStateCardTsarTurn roomStateCardTsarTurn)
    {
        _cardTsar = player;
        _room = room;
        _chosenWhiteCard = chosenWhiteCard;
        _roomStateCardTsarTurn = roomStateCardTsarTurn;

        _playersSubmitting = room.Players.Where(p => !p.Equals(_cardTsar)).ToList();
        _timer = new(_turnTime * 1000, TimerElapsed);
       
    }

    public async Task Enter(CancellationToken cancellationToken = default)
    {
        await _room.SendPlayer(_cardTsar, "CardTsarChoosing", _chosenWhiteCard, _turnTime, cancellationToken);
        await _room.SendAllExcept(_cardTsar, "PlayerCardChoosing", _cardTsar.ToPlayerDto(), _chosenWhiteCard, _turnTime, cancellationToken);

        _timer.StartTimer();
    }

    public async Task AddCrow(Player player, bool isReconnection, CancellationToken cancellationToken = default)
    {
        await _roomStateCardTsarTurn.AddCrow(player, isReconnection, cancellationToken);

        switch (isReconnection)
        {
            case false:
            case true when !_cardTsar.Equals(player) && !_playersSubmitting.Contains(player):
                _playersSubmitting.Add(player);
                break;
        }

        var turnTimeLeft = (int)(_timer.RemainingTime * 0.001);

        switch (isReconnection)
        {
            case true when player.Equals(_cardTsar):
                await _room.SendPlayer(_cardTsar, "CardTsarChoosing", _chosenWhiteCard, _turnTime, cancellationToken);
                break;
            default:
                await _room.SendPlayer(player, "PlayerChoosing", _cardTsar.ToPlayerDto(), cancellationToken);
                break;
        }

        foreach (var(crow, _) in _playerResults)
        {
            await _room.SendPlayer(crow, "SubmissionAccepted", player.ToPlayerDto(), null, cancellationToken);
        }

        foreach (var cm in _chatLog)
        {
            await _room.SendPlayer(player, "ChatMessage", cm, cancellationToken);
        }
    }

    public async Task RemoveCrow(Player player, CancellationToken cancellationToken = default)
    {
        await _roomStateCardTsarTurn.RemoveCrow(player, cancellationToken);

        if (!player.Equals(_cardTsar))
        {
            EndTurn();
            return;
        }

        _playersSubmitting.Remove(player);

        if (!_playersSubmitting.Any())
        {
            EndTurn();
        }
    }

    internal async Task SubmitCard(Player player, WhiteCard whiteCard, CancellationToken cancellationToken = default)
    {
        if (!_playersSubmitting.Contains(player))
        {
            var cm = new CrowChatMessage(CrowChatMessageType.AlreadyPlayedCard, "You already played a card", player.Name);
            _chatLog.Add(cm);
            await _room.SendAll("ChatMessage", cm, cancellationToken);
            return;
        }

        if (!await MoveSubmittingPlayerToResults(player, whiteCard, cancellationToken))
        {
            await ShowPlayedCards(player, whiteCard, cancellationToken);
            return;
        }

        EndTurn();
    }
    
    internal async Task UndoSelection(Player player, CancellationToken cancellationToken = default)
    {
        if (_cardTsar == player)
        {
            await _room.SendAllExcept(player, "undoing winning card", cancellationToken);
        }
    }

    private void EndTurn()
    {
        lock (Locking)
        {
            _turnEnded = true;
            _timer.Dispose();

            var cm = new CrowChatMessage(CrowChatMessageType.GameFlow, null, $"{_cardTsar.Name} chose .");

            _ = _room.SendAll("ChatMessage", cm);

            _room.RoomState = new RoomStateScoring(_cardTsar, _room, _chosenWhiteCard, _playerResults, _playersSubmitting, _roomStateCardTsarTurn);
        }
    }

    private void TimerElapsed(object? sender, ElapsedEventArgs e) => EndTurn();

    private async Task<bool> MoveSubmittingPlayerToResults(Player player, WhiteCard whiteCard, CancellationToken cancellationToken = default)
    {
        _playersSubmitting.Remove(player);
        _playerResults.Add(new(player, whiteCard));

        var timeRemaining = 0d;

        timeRemaining = _playerResults.Count <= 3
            ? _timer.ChangeTimerInterval(0.75)
            : _timer.RemainingTime;

        var turnTimeLeft = (int)(timeRemaining * 0.001);
        var whiteCardPlayedMessage =
        new CrowChatMessage(CrowChatMessageType.WhiteCardPlay, $"{player.Name} played a card", player.Name);
        await _room.SendAllExcept(player, "WhiteCardSubmitted", player.ToPlayerDto(), turnTimeLeft, null, whiteCardPlayedMessage,
        cancellationToken);
        await _room.SendPlayer(player, "WhiteCardSubmitted", player.ToPlayerDto(), turnTimeLeft, whiteCard, whiteCardPlayedMessage,
            cancellationToken);

        return _playersSubmitting.Count(p => p.IsConnected) is 0;
    }

    private async Task ShowPlayedCards(Player player, WhiteCard whiteCard,
        CancellationToken cancellationToken = default)
    {
        var displayCardsMessage = _room.RoomSettings.ShowPlayedCards
            ? new CrowChatMessage(CrowChatMessageType.WhiteCardPlay, whiteCard.CardText, player.Name)
            : new CrowChatMessage(CrowChatMessageType.WhiteCardPlay, null, player.Name);

        _chatLog.Add(displayCardsMessage);
        await _room.SendAll("ChatMessage", displayCardsMessage, cancellationToken);
    }
}
