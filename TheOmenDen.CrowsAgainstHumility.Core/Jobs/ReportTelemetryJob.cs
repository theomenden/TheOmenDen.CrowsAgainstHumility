using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Hosting;
using System.Timers;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Stores;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Settings;
using TheOmenDen.CrowsAgainstHumility.Core.Providers;

namespace TheOmenDen.CrowsAgainstHumility.Core.Jobs;
public class ReportTelemetryJob : IHostedService, IDisposable, IAsyncDisposable
{
    private const string ReportSessions = "Sessions";
    private const string ReportPlayers = "Players";

    private System.Timers.Timer? _timer;
    private readonly IServerStore _serverStore;
    private readonly TelemetryClient _telemetryClient;
    private readonly ICrowGameConfiguration _crowGameConfiguration;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ReportTelemetryJob(IServerStore serverStore, TelemetryClient telemetryClient, ICrowGameConfiguration crowGameConfiguration, IDateTimeProvider? dateTimeProvider)
    {
        _serverStore = serverStore;
        _telemetryClient = telemetryClient;
        _crowGameConfiguration = crowGameConfiguration;
        _dateTimeProvider = dateTimeProvider ?? DateTimeProvider.Default;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var timerInterval = _crowGameConfiguration.ClientInactivityCheckInterval;
        _timer = new System.Timers.Timer(timerInterval.TotalMilliseconds);
        _timer.Elapsed += MetricsOnElapsed;
        _timer.Start();

        await Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Dispose();
        return Task.CompletedTask;
    }

    private async void MetricsOnElapsed(object? sender, ElapsedEventArgs e)
    {
        await RunPlayerMetricsReportAsync(CancellationToken.None);
        await RunSessionMetricsReportAsync(CancellationToken.None);
    }

    private async Task RunPlayerMetricsReportAsync(CancellationToken cancellationToken)
    {
        var allPlayers = await _serverStore.GetTotalPlayersAsync(cancellationToken);
        _telemetryClient.TrackMetric(ReportPlayers, allPlayers);
    }
    private async Task RunSessionMetricsReportAsync(CancellationToken cancellationToken)
    {
        var allGames = await _serverStore.GetTotalSessionsAsync(cancellationToken);
        _telemetryClient.TrackMetric(ReportSessions, allGames);
    }
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

    public ValueTask DisposeAsync()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
    #endregion
}
