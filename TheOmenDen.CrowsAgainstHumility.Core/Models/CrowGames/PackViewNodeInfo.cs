using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;
public sealed class PackViewNodeInfo
{
    public PackViewNodeInfo(){}

    public PackViewNodeInfo(Guid id, NodeTypes nodeType, string text)
    {
        Id = id;
        NodeType = nodeType;
        Text = text;
    }

    public Guid Id { get; } = Guid.Empty;
    public NodeTypes NodeType { get; } = NodeTypes.Pack;
    public string Text { get; } = String.Empty;
    public IEnumerable<PackViewNodeInfo> Children { get; set; } = Enumerable.Empty<PackViewNodeInfo>();
}
