using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Cards;
using TheOmenDen.Shared.Specifications;

namespace TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Repositories;
public interface IPackRepository: IEnumerable<Pack>, IAsyncEnumerable<Pack>
{
    IAsyncEnumerable<Pack> GetAllPacksAsyncStream(CancellationToken cancellationToken = default);
    Task<IEnumerable<Pack>> GetAllPacksAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Pack>> GetAllPacksBySpecificationAsync(Specification<Pack> specification, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Pack> GetAllPacksThatMatch(Specification<Pack> specification, CancellationToken cancellationToken = default);
}
