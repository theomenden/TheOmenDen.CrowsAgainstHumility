using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Hosting;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Repositories;

namespace TheOmenDen.CrowsAgainstHumility.Services.Hosted;
public class ReportTelemetryJob : IHostedService, IDisposable, IAsyncDisposable
{
    #region Constants
    private const string ReportPlayersMetricName = "players";
    private const string ReportSessionsMetricName = "sessions";
    #endregion
    #region Private Members
    private Timer? _timer;
    private static readonly TimeSpan RunFrequency = TimeSpan.FromMinutes(10);
    #endregion
    #region Injected Services
    private readonly ICrowGameServerStore _serverStore;
    private readonly TelemetryClient _telemetryClient;
    #endregion
    #region Constructors
    public ReportTelemetryJob(ICrowGameServerStore serverStore, TelemetryClient telemetryClient)
    {
        _serverStore = serverStore;
        _telemetryClient = telemetryClient;
    }
    #endregion
    #region IHostedService Implementations
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(_ => RunJob(), null, TimeSpan.Zero, RunFrequency);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, Timeout.Infinite);
        return Task.CompletedTask;
    }
    #endregion
    #region Private Methods
    private void RunJob()
    {
        ReportSessions();
        ReportPlayers();
    }

    private void ReportSessions()
    {
        var allSessions = _serverStore.All().Count() ?? 0;
        _telemetryClient.TrackMetric(ReportSessionsMetricName, allSessions);
    }

    private void ReportPlayers()
    {
        var allPlayers = _serverStore.All().Sum(server => server.Players.Count());
        _telemetryClient.TrackMetric(ReportPlayersMetricName, allPlayers);
    }
    #endregion
    #region Disposal Methods
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

    public async ValueTask DisposeAsync()
    {
        if (_timer is not null)
        {
            await _timer.DisposeAsync();
        }
    }
    #endregion
}
