using System.Linq.Expressions;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Core.Rules;

namespace TheOmenDen.CrowsAgainstHumility.Api.Sagas;

public sealed class GameSagaOrchestrator(RuleEngine ruleEngine, GameEngine gameEngine, ILogger<GameSagaOrchestrator> logger)
{
    public async ValueTask<bool> ExecuteGameSagaAsync(GameSessionDto session, CancellationToken cancellationToken = default)
    {
        try
        {
            await gameEngine.SetupGame(session, cancellationToken);
            while (!gameEngine.IsGameOver)
            {
                await ExecutePlayPhaseAsync(session, cancellationToken);
            }
            await gameEngine.FinalizeGameAsync(session, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while executing the game saga: {Message}", ex.Message);
            return false;
        }
    }

    private async ValueTask ExecutePlayPhaseAsync(GameSessionDto session, CancellationToken cancellationToken = default)
    {
        var tasks = session.Players.Select(player =>
            gameEngine.ProcessPlayerMoveAsync(session.Id, player.Id, player.NextMove, cancellationToken));
        await Task.WhenAll(tasks);
        await ruleEngine.ApplyRoundRules(session, cancellationToken);
        await gameEngine.UpdateGameStateAsync(session, cancellationToken);
    }
}
