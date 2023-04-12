using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;
using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Repositories;
public interface ICrowGameServerStore : IEnumerable<CrowGameServer>, IAsyncEnumerable<CrowGameServer>
{
    IEnumerable<string> LobbyNames { get; }
    #region Async CRUD
    Task<CrowGameServer> CreateAsync(Deck deck, CancellationToken cancellationToken = default);
    Task<CrowGameServer> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CrowGameServer> GetByLobbyCodeAsync(String lobbyCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<CrowGameServer>> GetAllAsync(CancellationToken cancellationToken = default);
    Task RemoveAsync(CrowGameServer server, CancellationToken cancellationToken = default);
    #endregion
}
