using TheOmenDen.CrowsAgainstHumility.Core.DTO.Models.Players;

namespace TheOmenDen.CrowsAgainstHumility.Services.Locks;

public interface ILobbyLock : IDisposable, IAsyncDisposable
{
    Lobby Lobby { get; }
    void Lock();
    Task LockAsync(CancellationToken cancellationToken = default);
}

internal class PlayerListLock: ILobbyLock
{
    private readonly SemaphoreSlim _semaphore = new (1,1);
    private readonly object _lockObject;
    private bool _isLocked;

    public PlayerListLock(Lobby players, object lockObject)
    {
        Lobby = players;
        _lockObject = lockObject;
    }

    public Lobby Lobby { get; private set; }


    public void Lock()
    {
        if (_isLocked)
        {
            return;
        }

        Monitor.TryEnter(_lockObject, 10_000, ref _isLocked);
        if (!_isLocked)
        {
            throw new TimeoutException(Resources.PlayerListTimeout);
        }
    }

    public async Task LockAsync(CancellationToken cancellationToken = default)
    {
        if (_isLocked)
        {
            await Task.CompletedTask;
            return;
        }

        using var semaphore = new SemaphoreSlim(1, 1);

        try
        {
            _isLocked = await semaphore.WaitAsync(10_000,cancellationToken);
        }
        finally
        {
            semaphore.Release();
        }
    }

    public void Dispose()
    {
        Dispose(disposing:true);
        GC.SuppressFinalize(this);
    }

    public ValueTask DisposeAsync()
    {
        Dispose(disposing:true);
        return ValueTask.CompletedTask;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_isLocked)
        {
            return;
        }

        if (!disposing)
        {
            return;
        }

        Monitor.Exit(_lockObject);
        _semaphore.Dispose();
    }
}
