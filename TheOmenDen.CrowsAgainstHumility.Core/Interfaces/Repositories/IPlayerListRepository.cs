using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;

namespace TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Repositories;
public interface IPlayerListRepository : IEnumerable<PlayerList>, IAsyncEnumerable<PlayerList>
{
    #region Asynchronous Methods
    IAsyncEnumerable<String> PLayerListNamesAsAsyncStream { get; }
    Task<PlayerList> LoadPlayerListAsync(string listName, CancellationToken cancellationToken = default);
    Task SavePlayerListAsync(PlayerList playerList, CancellationToken cancellationToken = default);
    Task DeletePlayerListAsync(PlayerList playerList, CancellationToken cancellationToken = default);
    Task DeleteExpiredPlayerListsAsync(CancellationToken cancellationToken = default);
    Task DeleteAllAsync(CancellationToken cancellationToken = default);
    #endregion
}
