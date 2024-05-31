using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Core.Rules;

public sealed class RuleEngine
{
    private readonly SortedList<int, IRule> _rules = new();

    public void AddRule(IRule rule) => _rules.Add(rule.Priority, rule);


    public async ValueTask ApplyRoundRules(GameContext context, CancellationToken cancellationToken = default)
    {
        foreach (var rule in _rules)
        {
            if (await rule.Value.EvaluateAsync(context, cancellationToken))
            {
                await rule.Value.ExecuteAsync(context, cancellationToken);
            }
        }
    }
}