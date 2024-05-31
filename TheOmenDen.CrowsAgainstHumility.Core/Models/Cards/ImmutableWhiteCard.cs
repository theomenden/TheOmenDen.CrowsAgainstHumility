using TheOmenDen.CrowsAgainstHumility.Core.Identifiers;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models.Cards;

public sealed record ImmutableWhiteCard(CardId Id, string Text) : BaseImmutableCard(Id);