using System.Timers;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Services.Helpers;

namespace TheOmenDen.CrowsAgainstHumility.Services.Rooms;
internal sealed class RoomStateScoring: ICrowRoomState
{
    private readonly Player _cardTsar;
    private readonly CrowGameRoom _room;
    private readonly WhiteCard _chosenWhiteCard;
    private readonly List<Player> _playersSubmitting;
    private readonly RoomStateCardCzarTurn _roomStateCardCzarTurn;
    private readonly CrowGameTimer _turnEndTimer;
    private readonly List<(Player player, WhiteCard chosenCard)> _playerResults;
    private readonly int _timeout;

    private List<(Player player, Int32 score)> _playerScores;

    public RoomStateScoring(Player player, CrowGameRoom room, WhiteCard chosenWhiteCard, List<ValueTuple<Player, WhiteCard>> playerResults, List<Player> playersSubmitting, RoomStateCardCzarTurn roomStateCardCzarTurn)
    {
        _room = room;
        _chosenWhiteCard = chosenWhiteCard;
        _playersSubmitting = playersSubmitting;
        _playerResults = playerResults;
        _roomStateCardCzarTurn = roomStateCardCzarTurn;
        _cardTsar = player;
        CalculateScores();
        _timeout = 10 + _playerScores!.Count;
        _turnEndTimer = new(_timeout * 1000, TurnEndTimerElapsed);
    }

    public async Task Enter(CancellationToken cancellationToken = default)
    {
        await SendScores(_timeout, cancellationToken);
        _roomStateCardCzarTurn.Scores = _playerScores;
        _turnEndTimer.StartTimer();
    }

    public async Task AddCrow(Player player, bool isReconnection, CancellationToken cancellationToken = default)
    {
        await _roomStateCardCzarTurn.AddCrow(player, isReconnection, cancellationToken);
        
        var turnScores = _playerScores.Select(s => new PlayerScore(s.player.ToPlayerDto(), s.score)).ToArray();

        var timeRemaining = (int)(_turnEndTimer.RemainingTime * 0.001);

        await _room.SendPlayer(player, "TurnScores", turnScores, timeRemaining, cancellationToken);
    }

    public Task RemoveCrow(Player player, CancellationToken cancellationToken = default) => Task.CompletedTask;

    private void TurnEndTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        _turnEndTimer.Dispose();
        _room.RoomState = _roomStateCardCzarTurn;
    }

    private void CalculateScores()
    {
        _playerScores = new List<(Player player, int score)>(10);

        foreach (var p in _playersSubmitting)
        {
            _playerScores.Add(new(p, 0));
        }

        var (player, _) = _playerResults.First(p => p.chosenCard == _chosenWhiteCard);

        _playerScores.Add(new(player, 1));
    }

    private async Task SendScores(int timeout, CancellationToken cancellationToken = default)
    {
        var turnScores = _playerScores.Select(s => new PlayerScore(s.player.ToPlayerDto(), s.score)).ToArray();

        await _room.SendAll("TurnScores", turnScores, _chosenWhiteCard, timeout, cancellationToken);
    }
}
