using System.Timers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Engines;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Repositories;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Settings;

namespace TheOmenDen.CrowsAgainstHumility.Services.Hosted;
internal sealed class CleanupServerJob : IHostedService, IDisposable, IAsyncDisposable
{
    #region Injected Services
    private readonly ICrowGame _crowGame;
    private readonly IPlayerListRepository _playerListRepository;
    private readonly ICrowGameConfiguration _configuration;
    private readonly ILogger<CleanupServerJob> _logger;
    #endregion
    #region Private Members
    private System.Timers.Timer? _cleanupTimer;
    #endregion
    #region Constructors
    public CleanupServerJob(ICrowGame crowGame, IPlayerListRepository playerListRepository, ICrowGameConfiguration configuration, ILogger<CleanupServerJob> logger)
    {
        _logger = logger;
        _crowGame = crowGame ?? throw new ArgumentNullException(nameof(crowGame));
        _playerListRepository = playerListRepository ?? throw new ArgumentNullException(nameof(playerListRepository));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }
    #endregion
    #region IHosted Service Implementations
    public Task StartAsync(CancellationToken cancellationToken)
    {
        var timerInterval = _configuration.ClientInactivityCheckInterval;

        _cleanupTimer = new System.Timers.Timer(timerInterval.TotalMilliseconds);
        _cleanupTimer.Elapsed += OnCleanupTimerElapsed;
        _cleanupTimer.Start();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Dispose();
        return Task.CompletedTask;
    }
    #endregion
    #region Private Methods
    private void OnCleanupTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        DisconnectInactiveMembers();
        DeleteExpiredLobbies();
    }

    private void DisconnectInactiveMembers()
    {
        try
        {
            _crowGame.DisconnectInactiveObservers();
        }
        catch (Exception e)
        {
            _logger.LogError("Could not cleanup Crow Game: {CrowGame} due to exception: {@Ex}", nameof(_crowGame), e);
        }
    }

    private void DeleteExpiredLobbies()
    {
        try
        {
            _playerListRepository.DeleteExpiredLobbies();
        }
        catch (Exception e)
        {
            _logger.LogError("Could not delete Expired Lobbies due to: {@Ex}", e);
        }
    }
    #endregion
    #region Disposal Methods
    public void Dispose()
    {
        if (_cleanupTimer is not null)
        {
            _cleanupTimer.Dispose();
            _cleanupTimer = null;
        }
        GC.SuppressFinalize(this);
    }

    public ValueTask DisposeAsync()
    {
        if (_cleanupTimer is null)
        {
            return ValueTask.CompletedTask;
        }

        _cleanupTimer.Dispose();
        _cleanupTimer = null;
        return ValueTask.CompletedTask;
    }
    #endregion
}
