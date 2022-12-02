using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Services.Rooms;
internal sealed class RoomStateCardCzarTurn: ICrowRoomState
{
    private readonly Player _player;
    private readonly CrowGameRoom _room;
    private readonly CrowGameRound _roomRoundState;
    private int _entryCount = 0;

    public RoomStateCardCzarTurn(Player player, CrowGameRoom room, CrowGameRound roomRoundState)
    {
        _player= player;
        _room= room;
        _roomRoundState= roomRoundState;
    }

    public List<ValueTuple<Player, Int32>> Scores { get; internal set; } = Enumerable.Empty<ValueTuple<Player, Int32>>().ToList();

    public async Task Enter(CancellationToken cancellationToken = default)
    {
        switch (_entryCount)
        {
            case 0:
                await StartPlayerTurn();
                break;
            case 1:
                await EndPlayerTurn();
                break;
            default: throw new InvalidOperationException($"Unknown entryCount value: {_entryCount}.");
        }

        _entryCount++;
    }

    public async Task AddCrow(Player player, bool isReconnection, CancellationToken cancellationToken = default)
    {
        await _roomRoundState.AddCrow(player, isReconnection, cancellationToken);
        await _room.SendPlayer(player, "ChatMessage",
            new CrowChatMessage(CrowChatMessageType.GameFlow, null, $"{player.Name}'s turn to play"), cancellationToken);
    }

    public Task RemoveCrow(Player player, CancellationToken cancellationToken = default)
    => Task.CompletedTask;

    private async Task EndPlayerTurn()
    {
        await _roomRoundState.AddScore(Scores);
        _room.RoomState = _roomRoundState;
    }

    //TODO: Adjust new reference to actual parameter
    private async Task StartPlayerTurn()
    {
        await _room.SendAll("ChatMessage",
            new CrowChatMessage(CrowChatMessageType.GameFlow, null, $"{_player.Name}'s turn to be the Card Tsar."));

        _room.RoomState = new RoomStateBlackCard(_player, _room, new(), this); 
    }
}
