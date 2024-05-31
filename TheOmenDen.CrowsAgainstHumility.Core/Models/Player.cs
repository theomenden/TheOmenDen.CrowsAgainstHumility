using TheOmenDen.CrowsAgainstHumility.Core.Enums;
using TheOmenDen.CrowsAgainstHumility.Core.Identifiers;
using TheOmenDen.CrowsAgainstHumility.Core.Models.Cards;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models;

public class Player(Guid corvidOnlineId, string username)
{
    public Guid CorvidOnlineId { get; private set; } = corvidOnlineId;
    public PlayerId Id { get; private set; } = PlayerId.New();
    public string Username { get; private set; } = username;
    public List<ImmutableWhiteCard> Hand { get; private set; } = [];
    public GameRole CurrentRole { get; private set; } = GameRole.Player;
    public int Score { get; private set; }

    public void SetGameRole(GameRole role) => CurrentRole = role;

    public void DrawWhiteCard(ImmutableWhiteCard card) => Hand.Add(card);

    public bool PlayWhiteCard(ImmutableWhiteCard card) => Hand.Remove(card);

    public void AwardPoint() => Score++;
}