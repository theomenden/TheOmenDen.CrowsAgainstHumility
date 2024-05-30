using TheOmenDen.CrowsAgainstHumility.Core.Identifiers;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models.Cards;

public class Expansion
{
    public ExpansionId Id { get; set; } = ExpansionId.New();
    public string Name { get; set; }
    public string Description { get; set; }
    public ICollection<WhiteCard> WhiteCards { get; set; } = [];
    public ICollection<BlackCard> BlackCards { get; set; } = [];
}