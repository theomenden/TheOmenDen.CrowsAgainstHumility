using Microsoft.Extensions.Hosting;

namespace TheOmenDen.CrowsAgainstHumility.Services.Hosted;
internal class CleanupServerJob: IHostedService, IDisposable
{
    private Timer? _timer;
    private static readonly TimeSpan RunFrequency = TimeSpan.FromMinutes(20);
    private readonly IServerStore _serverStore;

    public CleanupServerJob(IServerStore serverStore) => _serverStore = serverStore;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(_ => RunJobAsync(cancellationToken), null, TimeSpan.Zero, RunFrequency);
        return Task.CompletedTask;
    }

    private void RunJob(CancellationToken cancellationToken)
    {
        var createdThreshold = DateTime.UtcNow.Subtract(TimeSpan.FromHours(1));

        foreach (var server in _serverStore.Servers)
        {
            var isOld = createdThreshold > server.Created;
            var isEmptyOrAsleep = server.Players.All( p => !p.IsAlive);

            if (isOld && isEmptyOrAsleep)
            {
                _serverStore.Remove(server);
            }
        }
    }

    private Task RunJobAsync(CancellationToken cancellationToken)
    {
        var createdThreshold = DateTime.UtcNow.Subtract(TimeSpan.FromHours(1));

        foreach (var server in _serverStore.Servers)
        {
            var isOld = createdThreshold > server.Created;
            var isEmptyOrAsleep = server.Players.All(p => !p.IsAlive);

            if (isOld && isEmptyOrAsleep)
            {
                _serverStore.Remove(server);
            }
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, Timeout.Infinite);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _timer?.Dispose();
        }
    }
}
