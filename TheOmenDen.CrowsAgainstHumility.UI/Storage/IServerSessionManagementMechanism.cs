namespace TheOmenDen.CrowsAgainstHumility.Storage;

public interface IServerSessionManagementMechanism
{
    ValueTask<(bool couldGet, ServerSession? session)> TryGetSessionAsync(string serverId, CancellationToken cancellationToken = default);
    Task SetSessionAsync(ServerSession session, CancellationToken cancellationToken = default);
    Task RemoveSessionAsync(string sessionId, CancellationToken cancellationToken = default);
}