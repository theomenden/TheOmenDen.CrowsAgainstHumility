using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Core.Rules;

public sealed record TimerRule(TimeSpan Duration, int Priority = 10) : Rule(Priority)
{
    public override async ValueTask<bool> EvaluateAsync(GameContext context, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(context.ShouldApplyTimer());
    }

    public override async ValueTask ExecuteAsync(GameContext context, CancellationToken cancellationToken = default)
    {
        context.StartTimer(Duration);
        await Task.CompletedTask;
    }
}