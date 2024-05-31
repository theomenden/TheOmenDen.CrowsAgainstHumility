using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Core.Rules;

public record ScoringRule(int Priority = 1) : Rule
{
    public override async ValueTask<bool> EvaluateAsync(GameContext context, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(context.CurrentPhase == GamePhase.Scoring);
    }

    public override async ValueTask ExecuteAsync(GameContext context, CancellationToken cancellationToken = default)
    {
        var winningPlayer = context.FindPlayerByCard(context.WinningCard);

        if (winningPlayer is not null)
        {
            winningPlayer.Score++;
        }

        await Task.CompletedTask;
    }
}