using TheOmenDen.CrowsAgainstHumility.Core.DTO.ViewModels;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Game;

namespace TheOmenDen.CrowsAgainstHumility.Core.Engine.Stores;
public interface IServerStore
{
    Task<CrowGameServer> CreateServerAsync(CreateCrowGameViewModel configuration, CancellationToken cancellationToken = default);
    Task<CrowGameServer> GetServerByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CrowGameServer> GetServerByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<int> GetTotalSessionsAsync(CancellationToken cancellationToken = default);
    Task<int> GetTotalPlayersAsync(CancellationToken cancellationToken = default);
    IAsyncEnumerable<CrowGameServer> GetAllServersAsyncStream(CancellationToken cancellationToken = default);
    Task<bool> RemoveServerAsync(CrowGameServer serverToRemove, CancellationToken cancellationToken = default);
    IAsyncEnumerable<bool> RemoveAllServersAsyncStream(CancellationToken cancellationToken = default);
}
