using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;

namespace TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Repositories;
public interface IPlayerListRepository : IEnumerable<PlayerList>, IAsyncEnumerable<PlayerList>
{
    #region Synchronous Methods
    IEnumerable<string> LobbyNames { get; }
    PlayerList? LoadPlayerList(string lobbyName);
    void SavePlayerList(PlayerList playerList);
    void DeletePlayerList(string lobbyName);
    void DeleteExpiredLobbies();
    void DeleteAll();
    #endregion
    #region Asynchronous Methods
    IAsyncEnumerable<String> PLayerListNamesAsAsyncStream { get; }
    Task<PlayerList> LoadPlayerListAsync(string listName, CancellationToken cancellationToken = default);
    Task SavePlayerListAsync(PlayerList playerList, CancellationToken cancellationToken = default);
    Task DeletePlayerListAsync(PlayerList playerList, CancellationToken cancellationToken = default);
    Task DeleteExpiredPlayerListsAsync(CancellationToken cancellationToken = default);
    Task DeleteAllAsync(CancellationToken cancellationToken = default);
    #endregion
}
