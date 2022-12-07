using System.Timers;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Services.Helpers;
using TheOmenDen.Shared.Utilities;

namespace TheOmenDen.CrowsAgainstHumility.Services.Rooms;

internal sealed class RoomStateWinningWhiteCard: ICrowRoomState
{
    private static readonly object LockingObject = new();
    private const int WhiteCardChoiceTimeout = 300;

    private readonly Player _cardTsar;
    private readonly CrowGameRoom _room;
    private readonly RoomStateCardCzarTurn _roomStateCardCzarTurn;

    private readonly List<WhiteCard> _suppliedWhiteCards = new(10);
    private readonly CrowGameTimer _readingTimer;

    private bool _hasWhiteCardBeenChosen = false;

    public RoomStateWinningWhiteCard(Player player, CrowGameRoom room, RoomStateCardCzarTurn roomStateCardCzarTurn)
    {
        _cardTsar= player;
        _room= room;
        _roomStateCardCzarTurn= roomStateCardCzarTurn;

        _readingTimer = new(WhiteCardChoiceTimeout * 1000, WhiteCardChoiceTimerElapsed);
    }

    public async Task Enter(CancellationToken cancellationToken = default)
    {
        await _room.SendPlayer(_cardTsar, "TsarWhiteCardChoice", null, WhiteCardChoiceTimeout, cancellationToken);
        await _room.SendAllExcept(_cardTsar, "PlayerwordChoice", _cardTsar.ToPlayerDto(), WhiteCardChoiceTimeout, cancellationToken);

        _readingTimer.StartTimer();
    }

    public async Task AddCrow(Player player, bool isReconnection, CancellationToken cancellationToken = default)
    {
        var timeRemaining = (int)(_readingTimer.RemainingTime * 0.001);
        if (isReconnection && player == _cardTsar)
        {
            await _room.SendPlayer(_cardTsar, "CardTsarWhiteCardChoice", WhiteCardChoiceTimeout, cancellationToken);
            return;
        }

        await _room.SendPlayer(player, "PlayerWhiteCardChoice", _cardTsar.ToPlayerDto(), timeRemaining, cancellationToken);
    }

    public Task RemoveCrow(Player player, CancellationToken cancellationToken = default)
    {
        if (player == _cardTsar)
        {
            _room.RoomState = _roomStateCardCzarTurn;
        }

        return Task.CompletedTask;
    }

    internal void WhiteCardChosen(Int32 whiteCardIndex, Player player)
    {
        lock (LockingObject)
        {
            if (_hasWhiteCardBeenChosen)
            {
                return;
            }

            _hasWhiteCardBeenChosen = true;
            _readingTimer.Dispose();

            if (!_cardTsar.Equals(player))
            {
                return;
            }

            var threadSafeRandom = ThreadSafeRandom.Global.Next(_suppliedWhiteCards.Count - 1);

            var chosenWhiteCard = _suppliedWhiteCards.ElementAtOrDefault(whiteCardIndex)
                ?? _suppliedWhiteCards.ElementAt(threadSafeRandom);

            var rejectedCards = _suppliedWhiteCards.Where(wc => wc != chosenWhiteCard).ToArray();

            _room.AddRejectedWhiteCardsToPlayedWhiteCards(rejectedCards);

            _room.RoomState = new RoomStateBlackCard(player, _room, chosenWhiteCard, _roomStateCardCzarTurn);
        }
    }

    private void WhiteCardChoiceTimerElapsed(object? sender, ElapsedEventArgs e)
    => WhiteCardChosen(1, _cardTsar);
    

}
