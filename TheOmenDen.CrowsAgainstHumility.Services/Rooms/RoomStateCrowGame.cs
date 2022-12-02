using System.Timers;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using Timer = System.Threading.Timer;

namespace TheOmenDen.CrowsAgainstHumility.Services.Rooms;
internal class RoomStateCrowGame: ICrowRoomState
{
    private int _entryCount = 0;
    private readonly CrowGameRoom _room;
    private readonly CrowRoomStateLobby _roomStateLobby;
    private Dictionary<Player, int> _playerScores = new(100);
    private System.Timers.Timer? _gameEndTimer;

    public RoomStateCrowGame(CrowGameRoom crowGameRoom, CrowRoomStateLobby rsl)
    {
        _room= crowGameRoom;
        _roomStateLobby= rsl;
    }

    public async Task Enter(CancellationToken cancellationToken = default)
    {
        if (_entryCount is 0)
        {
            await _room.SendAll("GameStarted", cancellationToken);
        }

        if (_room.Players.Any(p => p.IsConnected))
        {
            _room.RoomState= _roomStateLobby;
            return;
        }

        if (_entryCount < _room.RoundCount)
        {
            _room.RoomState = new CrowGameRound(_entryCount, _room, this);
            _entryCount++;
            return;
        }

        var timeout = 12 + (2 * _playerScores.Count);
        await GameScores(timeout);

        _gameEndTimer = new System.Timers.Timer(timeout * 1000);
        _gameEndTimer.Elapsed += GameEndTimerElapsed;
        _gameEndTimer.AutoReset = false;
        _gameEndTimer.Start();

        _entryCount++;
    }

    public async Task AddCrow(Player player, bool isReconnection, CancellationToken cancellationToken = default)
    {
        await _room.SendPlayer(player, "GameStarted", cancellationToken);
        var totalScores = GetTotalScores().ToArray();
        await _room.SendPlayer(player, "UpdateTotalScores", totalScores, cancellationToken);
    }

    public Task RemoveCrow(Player player, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    internal Task AddScore(List<ValueTuple<Player,int>> scores)
    {
        lock (_playerScores)
        {
            foreach (var (player, score) in scores)
            {
                if (_playerScores.TryGetValue(player, out var currentScore))
                {
                    _playerScores.Remove(player);
                    _playerScores.Add(player, currentScore);
                    continue;
                }

                _playerScores.Add(player, score);
            }
        }
        return SendTotalScores();
    }

    private void GameEndTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        _gameEndTimer?.Stop();
        _gameEndTimer?.Dispose();

        _room.RoomState = _roomStateLobby;

        _ = _room.SendAll("GameEnded");
    }

    private Task SendTotalScores()
    {
        var totalScores = GetTotalScores().ToArray();
        return _room.SendAll("UpdateTotalScores", totalScores);
    }

    private Task GameScores(Int32 timeout)
    {
        var totalScores = GetTotalScores().ToArray();
        return _room.SendAll("GameScores", totalScores, timeout);
    }

    private IEnumerable<PlayerScore> GetTotalScores()
    {
        List<PlayerScore> totalScores;
        lock (_playerScores)
        {
            totalScores = _playerScores.Select(s => new PlayerScore(s.Key.ToPlayerDto(), s.Value)).ToList();
        }
        return totalScores;
    }
}
