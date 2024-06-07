using TheOmenDen.CrowsAgainstHumility.Core.Enums;

namespace TheOmenDen.CrowsAgainstHumility.Api.Sagas;

public sealed class GameSagaOrchestrator(IGameStateService gameStateService, ILogger<GameSagaOrchestrator> logger)
{
    public async Task StartGameAsync(SessionId sessionId, CancellationToken cancellationToken = default)
    {
        var initialState = await gameStateService.InitializeGameStateAsync(sessionId, cancellationToken);

        await gameStateService.SaveGameStateAsync(initialState, cancellationToken);

        await NotifyPlayersAsync(sessionId, "Game has started", cancellationToken);

        await ExecuteGameLoopAsync(sessionId, cancellationToken);
    }

    private async Task ExecuteGameLoopAsync(SessionId sessionId, CancellationToken cancellationToken)
    {
        var gameIsRunning = true;
        while (gameIsRunning)
        {
            var gameState = await gameStateService.GetGameStateAsync(sessionId, cancellationToken);

            // Handle game round Logic
            await HandleRoundAsync(gameState);

            // Check if game should continue
            gameIsRunning = CheckGameForContinuation(gameState);
        }

        // Finalize Game and clean up
        await FinalizeGameAsync(sessionId, cancellationToken);
    }

    private async Task HandleRoundAsync(GameSessionDto gameState)
    {
        // Stub this out later
    }

    private bool CheckGameForContinuation(GameSessionDto gameState) => gameState.Status != GameStatus.Completed;

    private async Task FinalizeGameAsync(SessionId sessionId, CancellationToken cancellationToken)
    {
        await gameStateService.UpdateGameStatusAsync(sessionId, new PlayerAction { Type = ActionType.EndGame }, cancellationToken);

        await NotifyPlayersAsync(sessionId, "Game has ended", cancellationToken);

        await gameStateService.DeleteGameStateAsync(sessionId, cancellationToken);
    }


    private async Task NotifyPlayersAsync(SessionId sessionId, string message, CancellationToken cancellationToken)
    {
        var players = await gameStateService.GetPlayersAsync(sessionId, cancellationToken);

        // Notify Players via SignalR Hub
    }
}
