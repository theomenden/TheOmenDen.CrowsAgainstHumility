using TheOmenDen.CrowsAgainstHumility.Core.Enums;

namespace TheOmenDen.CrowsAgainstHumility.Core.Rules;

public sealed record GameRules(
    TimeSpan BlackCardReadingTime,
    TimeSpan WhiteCardSubmissionTime,
    TimeSpan JudgeSelectionTime,
    JudgeSelectionMethod JudgeSelectionMethod,
    int WinningPoints = 10
    );