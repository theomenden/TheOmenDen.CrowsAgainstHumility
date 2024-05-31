using System.Reflection.Metadata.Ecma335;
using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Core.Rules;

public record CardSubmissionRule(int Priority = 10) : Rule
{
    public override async ValueTask<bool> EvaluateAsync(GameContext context, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(context.CurrentPhase == GamePhase.CardSubmission);
    }

    public override async ValueTask ExecuteAsync(GameContext context, CancellationToken cancellationToken = default)
    {
        foreach (var player in context.Players)
        {
            if (context.HasSubmittedCard(player))
            {
                continue;
            }

            var cardToSubmit = player.Hand.GetRandomCard();
            await context.SubmitCardAsync(player, cardToSubmit, cancellationToken);
        }
    }
}