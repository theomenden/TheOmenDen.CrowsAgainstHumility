using System.Runtime.CompilerServices;
using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
public interface ICardPoolFilteringService
{
    Task<IEnumerable<WhiteCard>> GetDefaultFiltersOnOfficialWhiteCardsAsync(CancellationToken cancellationToken = default);
    IAsyncEnumerable<WhiteCard> GetDefaultFiltersOnOfficialWhiteCardsStreamAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<WhiteCard>> GetFilteredWhiteCardsAsync(String filterValue,
        IEnumerable<Guid> cardPackIds, CancellationToken cancellationToken = default);
    Task<IEnumerable<WhiteCard>> GetFilteredWhiteCardsAsync(IEnumerable<String> filterValues,
        IEnumerable<Guid> cardPackIds, CancellationToken cancellationToken = default);
}
