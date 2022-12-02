using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Services.CardPoolBuilding;

namespace TheOmenDen.CrowsAgainstHumility.Services.Rooms;
internal sealed class CrowGameRound: ICrowRoomState
{
    private readonly CrowGameRoom _room;
    private readonly RoomStateCrowGame _roomStateGame;
    private readonly List<Player> _playersThatHavePlayedACard = new(10);
    private int _entryCount;
    private int _roundNumber;

    public CrowGameRound(int roundNumber, CrowGameRoom room, RoomStateCrowGame roomStateGame)
    {
        _roundNumber= roundNumber;
        _room= room;
        _roomStateGame= roomStateGame;
    }

    public async Task Enter(CancellationToken cancellationToken = default)
    {
        if (_entryCount is 0)
        {
            var currentRound = _roundNumber + 1;

            var cm = new CrowChatMessage(CrowChatMessageType.GameFlow, null, $"Round {currentRound} starting.");

            await _room.SendAll("RoundStarted", currentRound, _room.RoomSettings.Rounds, cm, cancellationToken);
        }

        var remainingPlayers = _room.Players.Except(_playersThatHavePlayedACard).Where(p => p.IsConnected).ToArray();

        if (remainingPlayers.Any())
        {
            var player = remainingPlayers.First();
            _playersThatHavePlayedACard.Add(player);
            _room.RoomState = new RoomStateCardCzarTurn(player, _room, this);
            _entryCount++;
            ReplenishHands();
            return;
        }

        _room.RoomState = _roomStateGame;
        _entryCount++;
    }

    public async Task AddCrow(Player player, bool isReconnection, CancellationToken cancellationToken = default)
    {
        await _roomStateGame.AddCrow(player, isReconnection, cancellationToken);

        var currentRound = _roundNumber + 1;

        var cm = new CrowChatMessage(CrowChatMessageType.GameFlow, null, $"Round {currentRound} starting.");

        await _room.SendPlayer(player, "RoundStarted", currentRound, _room.RoomSettings.Rounds, cm, cancellationToken);
    }

    public Task RemoveCrow(Player player, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    internal Task AddScore(List<ValueTuple<Player, Int32>> scores)
    => _roomStateGame.AddScore(scores);

    private void ReplenishHands()
    {
        WhiteCardProvider.ReplenishPlayerHand(_room);
    }
}
