using TheOmenDen.CrowsAgainstHumility.Core.Identifiers;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models;

public class Player(PlayerId userId, string username)
{
    public PlayerId UserId { get; private set; } = userId;
    public string Username { get; private set; } = username;
    public List<Card> Hand { get; private set; } = [];
    public int Score { get; set; }
    public bool IsJudge { get; set; }
}