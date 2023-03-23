using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;

namespace TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
public interface IPackViewService
{
    IAsyncEnumerable<PackViewNodeInfo> GetPackViewNodeInfoAsyncStream(CancellationToken cancellationToken = default);
    Task<IEnumerable<PackViewNodeInfo>> GetPackViewNodeInfoAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Pack>> GetPacksForGameCreationAsync(CancellationToken cancellationToken = default);
}
