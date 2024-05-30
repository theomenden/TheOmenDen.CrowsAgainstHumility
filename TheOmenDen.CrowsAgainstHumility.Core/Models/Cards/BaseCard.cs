using TheOmenDen.CrowsAgainstHumility.Core.Identifiers;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models.Cards;

public abstract class BaseCard(string text)
{
    public CardId Id { get; private set; } = CardId.New();
    public string Text { get; private set; } = text;
    public ExpansionId ExpansionId { get; set; }
    public Expansion Expansion { get; set; }
}