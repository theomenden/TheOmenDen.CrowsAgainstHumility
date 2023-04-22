using System.Timers;
using Microsoft.Extensions.Hosting;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Stores;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Settings;
using TheOmenDen.CrowsAgainstHumility.Core.Providers;

namespace TheOmenDen.CrowsAgainstHumility.Core.Jobs;
public class CleanupServerJob : IHostedService, IDisposable, IAsyncDisposable
{
    #region Private Members
    private System.Timers.Timer? _timer;
    private readonly IServerStore _serverStore;
    private readonly ICrowGameConfiguration _crowGameConfiguration;
    private readonly IDateTimeProvider _dateTimeProvider;
    #endregion
    #region Constructors
    public CleanupServerJob(IServerStore serverStore, ICrowGameConfiguration configuration, IDateTimeProvider? dateTimeProvider)
    {
        ArgumentNullException.ThrowIfNull(serverStore);
        ArgumentNullException.ThrowIfNull(configuration);
        _dateTimeProvider = dateTimeProvider ?? DateTimeProvider.Default;
        _serverStore = serverStore;
        _crowGameConfiguration = configuration;
    }
    #endregion
    public Task StartAsync(CancellationToken cancellationToken)
    {
        var timerInterval = _crowGameConfiguration.ClientInactivityCheckInterval;
        _timer = new System.Timers.Timer(timerInterval.TotalMilliseconds);
        _timer.Elapsed += CleanupOnElapsed;
        _timer.Start();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Dispose();
        return Task.CompletedTask;
    }
    #region Private Methods

    private async void CleanupOnElapsed(object? sender, ElapsedEventArgs e)
    {
        await RunJobAsync(CancellationToken.None);
    }


    private async Task RunJobAsync(CancellationToken cancellationToken)
    {
        var createdThreshold = _dateTimeProvider.UtcNow - _crowGameConfiguration.RepositoryPlayerListExpiration;
        try
        {
            await foreach (var server in _serverStore.GetAllServersAsyncStream(cancellationToken))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                var isOldServer = createdThreshold > server.CreatedAt;
                var isEmptyOrAllAsleep = server.Players.All(p => p.Value.Mode != PlayerMode.Awake);

                if (isOldServer && isEmptyOrAllAsleep)
                {
                    await _serverStore.RemoveServerAsync(server, cancellationToken);
                }
            }
        }
        catch (Exception)
        {
            // We can always try again later :) 
        }
    }
    #endregion
    #region Disposal Methods
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public ValueTask DisposeAsync()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _timer?.Dispose();
        }
    }
    #endregion
}
