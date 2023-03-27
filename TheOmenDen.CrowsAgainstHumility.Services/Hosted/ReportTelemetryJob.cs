using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Abstractions;

namespace TheOmenDen.CrowsAgainstHumility.Services.Hosted;
public class ReportTelemetryJob : IHostedService, IDisposable, IAsyncDisposable
{
    #region Private members
    private Timer? _timer;
    private readonly IServerStore _serverStore;
    private readonly TelemetryClient _telemetryClient;
    #endregion
    #region Private Constants
    private static readonly TimeSpan RunFrequency = TimeSpan.FromMinutes(10);
    private const string ReportPlayersMetricName = "players";
    private const string ReportSessionsMetricName = "sesssions";
    #endregion
    #region Constructors
    public ReportTelemetryJob(IServerStore serverStore, TelemetryClient telemetryClient)
    {
        _serverStore = serverStore;
        _telemetryClient = telemetryClient;
    }
    #endregion
    #region Public Methods

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(_ => RunJob(), null, TimeSpan.Zero, RunFrequency);
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
        var allSessions = _serverStore.GetAll().Count();
        _telemetryClient.TrackMetric(ReportSessionsMetricName, allSessions);
    }

    private void ReportPlayers()
    {
        var allPlayers = _serverStore.GetAll().Sum(server => server.Players.Count());
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

    public ValueTask DisposeAsync()
    {
        _ = _timer?.DisposeAsync();
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
    #endregion

}
