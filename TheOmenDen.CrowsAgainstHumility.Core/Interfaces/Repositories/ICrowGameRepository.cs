using System.Runtime.CompilerServices;
using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Repositories;
public interface ICrowGameRepository
{
    Task CreateRoomAsync(String gameCode, String gameName, Guid userId, IEnumerable<Pack> packsToUse, CancellationToken cancellation = default);
    Task<IEnumerable<CrowGame>> GetCrowGamesAsync(CancellationToken cancellationToken = default);
    Task<CrowGame?> WithIdAsync(Guid id,CancellationToken cancellationToken = default);
    IAsyncEnumerable<CrowGame> GetCrowGameStreamAsync(CancellationToken cancellationToken = default);
}