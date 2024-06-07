using TheOmenDen.CrowsAgainstHumility.Core.Enums;
using TheOmenDen.CrowsAgainstHumility.Core.Identifiers;
using TheOmenDen.CrowsAgainstHumility.Core.Models.Cards;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models;

public sealed record GameSessionDto(
    SessionId Id,
    IReadOnlyList<PlayerState> Players,
    ImmutableBlackCard CurrentBlackCard,
    IReadOnlyDictionary<CardId, SortedList<int, ImmutableWhiteCard>> PlayerSubmissions,
    PlayerState CurrentJudge,
    GameStatus Status,
    TimeSpan BlackCardReadingTimerDuration,
    TimeSpan SubmissionTimerDuration,
    TimeSpan ReviewTimerDuration);