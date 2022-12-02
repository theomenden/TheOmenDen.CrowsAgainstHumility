using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Services.Rooms;
public sealed class CrowRoomStateLobby: ICrowRoomState
{
    private readonly CrowGameRoom _room;
    private readonly Func<CrowGameRoom, CancellationToken, Task> _gameEndCallback;
    private int _entryCount = 0;

    internal CrowRoomStateLobby(CrowGameRoom room, Func<CrowGameRoom, CancellationToken, Task> gameEndCallback)
    {
        _room= room;
        _gameEndCallback= gameEndCallback;
    }

    internal async Task<Boolean> SetRoomSettings(CrowRoomSettings settings, Player player)
    {
        var cardTsar = _room.Players.FirstOrDefault(p => p.IsCardCzar);

        if (cardTsar is null || cardTsar == player)
        {
            return false;
        }

        _room.RoomSettings = settings;
        await _room.SendAllExcept(player, "RoomStateChanged", _room.ToRoomStateDto());
        return true;

    }

    public async Task Enter(CancellationToken cancellationToken = default)
    {
        if (_entryCount > 0)
        {
            await _gameEndCallback(_room, cancellationToken);
        }

        _entryCount++;
        return;
    }

    public Task AddCrow(Player player, bool isReconnection, CancellationToken cancellationToken = default)
        => _room.SendPlayer(player, "RoomStateChanged", _room.ToRoomStateDto(), cancellationToken);

    public Task RemoveCrow(Player player, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
