using Blazored.SessionStorage;

namespace TheOmenDen.CrowsAgainstHumility.Storage;

public sealed class ServerSessionManagementMechanism : IServerSessionManagementMechanism
{
    private readonly ISessionStorageService _sessionStorage;
    private const string SessionStoreName = "ServerSessions";

    public ServerSessionManagementMechanism(ISessionStorageService sessionStorage)
    {
        _sessionStorage = sessionStorage;
    }

    public async ValueTask<(bool couldGet, ServerSession? session)> TryGetSessionAsync(string serverId, CancellationToken cancellationToken = default)
    {
        var sessions = await _sessionStorage.GetItemAsync<Dictionary<string, ServerSession>>(SessionStoreName, cancellationToken);

        return sessions is null 
               || !sessions.ContainsKey(serverId)
            ? new (false, null)
            : new (true, sessions[serverId]);
    }

    public async Task SetSessionAsync(ServerSession session, CancellationToken cancellationToken = default)
    {
        var sessions = await _sessionStorage.GetItemAsync<Dictionary<string, ServerSession>>(SessionStoreName, cancellationToken) ?? new Dictionary<string, ServerSession>();
        
        sessions[session.ServerId.ToString()] = session;
        await _sessionStorage.SetItemAsync(SessionStoreName, sessions, cancellationToken);
    }

    public async Task RemoveSessionAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sessionId);

        var sessions = await _sessionStorage.GetItemAsync<Dictionary<string, ServerSession>>(SessionStoreName, cancellationToken);
        sessions?.Remove(sessionId);
        await _sessionStorage.SetItemAsync(SessionStoreName, sessions, cancellationToken);
    }
}
