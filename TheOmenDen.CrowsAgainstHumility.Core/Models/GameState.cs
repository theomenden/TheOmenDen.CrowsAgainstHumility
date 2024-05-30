using TheOmenDen.CrowsAgainstHumility.Core.Enums;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models;

public class GameState
{
    public GameStatus Status { get; private set; } = GameStatus.WaitingToStart;
    public int CurrentPlayerIndex { get; private set; } = 0;
    public bool IsRoundComplete { get; set; } = false;

    public async Task StartGameAsync(CancellationToken cancellationToken = default)
    {
        Status = GameStatus.InProgress;
        await Status.HandleStateAsync(this);
    }
}