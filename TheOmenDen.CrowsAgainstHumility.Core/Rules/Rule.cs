using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Core.Rules;

public abstract record Rule(int Priority) : IRule
{
    public abstract ValueTask<bool> EvaluateAsync(GameContext context, CancellationToken cancellationToken = default);

    public abstract ValueTask ExecuteAsync(GameContext context, CancellationToken cancellationToken = default);
}