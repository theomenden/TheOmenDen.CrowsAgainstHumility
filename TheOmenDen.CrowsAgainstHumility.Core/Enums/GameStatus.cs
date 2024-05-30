using Ardalis.SmartEnum;
using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Core.Enums;

public abstract class GameStatus(string name, int value) : SmartEnum<GameStatus>(name, value)
{
    public static readonly GameStatus WaitingToStart = new WaitingToStartType();
    public static readonly GameStatus InProgress = new InProgressType();
    public static readonly GameStatus Paused = new PausedType();
    public static readonly GameStatus Completed = new CompletedType();

    public abstract Task HandleStateAsync(GameState context);


    private sealed class WaitingToStartType() : GameStatus("WaitingToStart", 1)
    {
        public override async Task HandleStateAsync(GameState context)
        {
            // Initialize the game
            await Task.CompletedTask;
        }
    }

    private sealed class InProgressType() : GameStatus("InProgress", 2)
    {
        public override async Task HandleStateAsync(GameState context)
        {
            // Check or Update the game asynchrously
            await Task.CompletedTask;
        }
    }

    private sealed class PausedType() : GameStatus("Paused", 3)
    {
        public override async Task HandleStateAsync(GameState context)
        {
            // Pause the game
            await Task.CompletedTask;
        }
    }

    private sealed class CompletedType() : GameStatus("Completed", 4)
    {
        public override async Task HandleStateAsync(GameState context)
        {
            // End the game
            await Task.CompletedTask;
        }
    }
}