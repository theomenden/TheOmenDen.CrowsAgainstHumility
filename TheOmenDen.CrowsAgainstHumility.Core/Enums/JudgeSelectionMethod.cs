using Ardalis.SmartEnum;
using TheOmenDen.CrowsAgainstHumility.Core.Identifiers;
using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Core.Enums;

public abstract class JudgeSelectionMethod(string name, int value) : SmartEnum<JudgeSelectionMethod>(name, value)
{
    public static readonly JudgeSelectionMethod Random = new RandomType();
    public static readonly JudgeSelectionMethod LastRoundWinner = new LastRoundWinnerType();
    public static readonly JudgeSelectionMethod NextPlayer = new NextPlayerType();

    public abstract PlayerId GetJudge(GameContext context);

    private sealed class RandomType() : JudgeSelectionMethod(nameof(Random), 1)
    {
        public override PlayerId GetJudge(GameContext context) => context.Players[new Random().Next(0, context.Players.Count)].PlayerId;
    }

    private sealed class LastRoundWinnerType() : JudgeSelectionMethod("Last Round's Winner", 2)
    {
        public override PlayerId GetJudge(GameContext context) => context.LastRoundWinner;
    }

    private sealed class NextPlayerType() : JudgeSelectionMethod("Next Player", 3)
    {
        public override PlayerId GetJudge(GameContext context)
        {
            var index = context.Players.FindIndex(p => p.PlayerId == context.CurrentJudge);
            return context.Players[(index + 1) % context.Players.Count].PlayerId;
        }
    }
}