using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Repositories;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;

namespace TheOmenDen.CrowsAgainstHumility.Services.Processing;

internal class PackViewProcessingService : IPackViewService
{
    private readonly IPackRepository _packs;
    private readonly ILogger<PackViewProcessingService> _logger;

    public PackViewProcessingService(IPackRepository packs, ILogger<PackViewProcessingService> logger)
    {
        _packs = packs;
        _logger = logger;
    }

    public async IAsyncEnumerable<PackViewNodeInfo> GetPackViewNodeInfoAsyncStream(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var pack in await _packs.GetAllPacksAsync(cancellationToken))
        {
            yield return new PackViewNodeInfo(pack.Id, NodeTypes.Pack, pack.Name)
            {
                Children = GetChildrenForPackView(pack)
            };
            _logger.LogInformation("Processed pack: {Pack}", pack.Name);
        }
    }

    public async Task<IEnumerable<PackViewNodeInfo>> GetPackViewNodeInfoAsync(
        CancellationToken cancellationToken = default)
    {
        var packs = (await _packs.GetAllPacksAsync(cancellationToken))
            .Select(pack =>
                new PackViewNodeInfo(pack.Id, NodeTypes.Pack, pack.Name)
                {
                    Children = GetChildrenForPackView(pack)
                }
            ).ToArray();
        return packs;
    }

    public async Task<IEnumerable<Pack>> GetPacksForGameCreationAsync(CancellationToken cancellationToken = default)
    {
        var packs = await _packs.GetAllPacksAsync(cancellationToken);

        return packs.ToArray();
    }

    private static IEnumerable<PackViewNodeInfo> GetChildrenForPackView(Pack pack)
    {
        var children = pack.WhiteCards.Select(card =>
            new PackViewNodeInfo(card.Id, NodeTypes.WhiteCard, card.CardText))
            .ToList();

        children.AddRange(pack.BlackCards.Select(card =>
            new PackViewNodeInfo(card.Id, NodeTypes.BlackCard, card.Message)));

        return children.ToArray();
    }
}
