using TheOmenDen.CrowsAgainstHumility.Core.Identifiers;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models.Cards;

public sealed record ImmutableBlackCard(CardId Id, string Text, int NumberOfBlanks) : BaseImmutableCard(Id);