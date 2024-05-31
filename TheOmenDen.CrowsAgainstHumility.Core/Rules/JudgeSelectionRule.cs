using TheOmenDen.CrowsAgainstHumility.Core.Enums;
using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Core.Rules;

public sealed record JudgeSelectionRule(JudgeSelectionMethod Method, int Priority = 5) : Rule(Priority)
{
    public override async ValueTask<bool> EvaluateAsync(GameContext context, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(context.CurrentPhase == GamePhase.JudgeSelection);
    }

    public override async ValueTask ExecuteAsync(GameContext context, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}