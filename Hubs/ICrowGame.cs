namespace TheOmenDen.CrowsAgainstHumility.Hubs;

public interface ICrowGame
{
    Task CreateGame(String username, String gameName, CancellationToken cancellationToken = default);
    Task JoinGame(String gameName, String username, CancellationToken cancellationToken = default);
    Task LeaveGame(String username, String gameName, CancellationToken cancellationToken = default);
}