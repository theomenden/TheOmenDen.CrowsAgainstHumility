using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.Shared.Specifications;

namespace TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Repositories;
public interface IPackRepository: IAsyncEnumerable<Pack>
{
    IAsyncEnumerable<Pack> GetAllPacksAsyncStream(CancellationToken cancellationToken = default);

    Task<IEnumerable<Pack>> GetAllPacksAsync(CancellationToken cancellationToken = default);

    IAsyncEnumerable<Pack> GetAllPacksThatMatch(Specification<Pack> specification, CancellationToken cancellationToken = default);
}
