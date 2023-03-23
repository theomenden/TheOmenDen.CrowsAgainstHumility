using System.Drawing.Printing;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;

namespace TheOmenDen.CrowsAgainstHumility.Components;

public partial class PackViewComponent : ComponentBase
{
    #region Private Members
    private IList<PackViewNodeInfo> _expandedNodes = new List<PackViewNodeInfo>(12);
    private PackViewNodeInfo _selectedNode = new ();
    private PackViewNodeInfo _childNode = new();
    private IList<PackViewNodeInfo> _nodes = new List<PackViewNodeInfo>(500);
    #endregion
    #region Injected Members
    [Inject] private IPackViewService PackViewService { get; init; }
    #endregion
    #region Life Cycle Methods
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _nodes = await PackViewService.GetPackViewNodeInfoAsyncStream().ToListAsync();
    }

    #endregion

    private Task<String> OnSelectedNode(string nodeId)
    {
        if (!Guid.TryParse(nodeId, out var id))
        {
            return Task.FromResult(String.Empty);
        }

        _selectedNode = _nodes.FirstOrDefault(n => n.Id == id) ?? new();

        return Task.FromResult(_selectedNode.Id.ToString());
    }

    private static String GetCountByCardType(PackViewNodeInfo parent)
    {
        var totalWhiteCardsInPack = parent.Children.Count(c => c.NodeType == NodeTypes.WhiteCard);
        var totalBlackCardsInPack = parent.Children.Count(c => c.NodeType == NodeTypes.BlackCard);

        return $"{totalWhiteCardsInPack} White Cards.{Environment.NewLine}{totalBlackCardsInPack} Black Cards.";
    }
    
}
