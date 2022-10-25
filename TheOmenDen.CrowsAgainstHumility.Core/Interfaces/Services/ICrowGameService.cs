using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
public interface ICrowGameService
{
    Task<IEnumerable<CrowGame>> GetCrowGamesAsync(CancellationToken cancellationToken = default);
    Task<CrowGameDto> GetCrowGameById(Guid id, CancellationToken cancellationToken = default);
    Task CreateCrowGameAsync(CrowGameCreator crowGameCreator, CancellationToken cancellationToken = default);
}
