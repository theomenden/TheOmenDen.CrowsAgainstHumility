using TheOmenDen.CrowsAgainstHumility.Core.Engine.Game;

namespace TheOmenDen.CrowsAgainstHumility.Core.Engine.Locks;
internal sealed class LobbyLock: ILobbyLock
{
    private readonly SemaphoreSlim _semaphore = new (1,1);
    private readonly CrowGameServer _gameServer;
    public LobbyLock(CrowGameServer server, object lockObject)
    {
        _gameServer = server;
    }

    public void Dispose()
    {
        _semaphore.Release();
    }

    public ValueTask DisposeAsync()
    {
        _semaphore.Release();
        return ValueTask.CompletedTask;
    }

    public ValueTask<CrowGameServer> GetGameServer() => ValueTask.FromResult(_gameServer);

    public async ValueTask LockAsync(CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        await ValueTask.CompletedTask;
    }
}
