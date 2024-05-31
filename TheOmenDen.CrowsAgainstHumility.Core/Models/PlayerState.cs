using TheOmenDen.CrowsAgainstHumility.Core.Enums;
using TheOmenDen.CrowsAgainstHumility.Core.Identifiers;
using TheOmenDen.CrowsAgainstHumility.Core.Models.Cards;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models;

public sealed record PlayerState
{
    public PlayerId PlayerId { get; init; }
    public string Username { get; init; }
    public int Score { get; init; }
    public GameRole Role { get; init; }
    public IReadOnlyList<ImmutableWhiteCard> Hand { get; init; } = [];

    public PlayerState(PlayerId playerId, string username, GameRole role, int score)
    {
        PlayerId = playerId;
        Username = username;
        Role = role;
        Score = score;
        Hand = [];
    }

    public PlayerState UpdateRole(GameRole role) => this with { Role = role };
    public PlayerState AddScore(int points = 0) => this with { Score = Score + points };
    public PlayerState AdjustHand(IEnumerable<ImmutableWhiteCard> newHand) => this with { Hand = newHand.ToList() };
}