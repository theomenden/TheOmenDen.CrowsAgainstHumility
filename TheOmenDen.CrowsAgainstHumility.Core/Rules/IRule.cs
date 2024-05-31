using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Core.Rules;

public interface IRule
{
    int Priority { get; }
    ValueTask<bool> EvaluateAsync(GameContext context, CancellationToken cancellationToken = default);
    ValueTask ExecuteAsync(GameContext context, CancellationToken cancellationToken = default);
}