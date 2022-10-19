using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
public interface ICardPoolBuildingService
{
    IAsyncEnumerable<Pack> GetPacksBySearchValueAsync(String searchValue, CancellationToken cancellationToken = default);

    IAsyncEnumerable<Pack> GetRandomPacksAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<Pack>> GetPacksByNameAsync(IEnumerable<String> packNames, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Pack>> GetOfficialPacksAsync(CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Pack>> GetOfficialPacksWithCardsAsync(CancellationToken cancellationToken = default);

}
