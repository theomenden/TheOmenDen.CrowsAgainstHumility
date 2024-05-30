using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Core.Rules;

public class RuleEngine
{
    private readonly List<IRule> _rules = new();

    public void AddRule(IRule rule)
    {
        _rules.Add(rule);
    }

    public async ValueTask ExecuteAsync(GameContext context, CancellationToken cancellationToken = default)
    {
        foreach (var rule in _rules.OrderBy(r => r.Priority))
        {
            if (await rule.EvaluateAsync(context, cancellationToken))
            {
                await rule.ExecuteAsync(context, cancellationToken);
            }
        }
    }
}