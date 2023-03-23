using TheOmenDen.Shared.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
public sealed record NodeTypes: EnumerationBase<NodeTypes>
{
    private NodeTypes(string name, int id) : base(name, id) {}

    public static readonly NodeTypes Root = new(nameof(Root), 1);
    public static readonly NodeTypes Pack = new(nameof(Pack), 2);
    public static readonly NodeTypes WhiteCard = new(nameof(WhiteCard), 3);
    public static readonly NodeTypes BlackCard = new(nameof(BlackCard), 4);
}
