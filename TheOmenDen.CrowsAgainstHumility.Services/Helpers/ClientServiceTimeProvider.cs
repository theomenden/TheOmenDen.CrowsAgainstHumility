using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;

namespace TheOmenDen.CrowsAgainstHumility.Services.Helpers;
internal sealed class ClientServiceTimeProvider: IServiceTimeProvider
{
    private static readonly TimeSpan UpdateInterval = TimeSpan.FromMinutes(5);

    private readonly ICrowGameClient _crowGameClient;
    private readonly ILogger<ClientServiceTimeProvider> _logger;
    private readonly DateTimeProvider _dateTimeProvider;
    private TimeSpan _serviceTimeOffset;
    private DateTime _lastUpdateTime;

    public ClientServiceTimeProvider(ICrowGameClient crowGameClient, ILogger<ClientServiceTimeProvider> logger, DateTimeProvider dateTimeProvider)
    {
        _crowGameClient = crowGameClient;
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
    }

    public TimeSpan ServiceTimeOffset => _serviceTimeOffset;

    public async Task UpdateServiceTimeOffset(CancellationToken cancellationToken = default)
    {
        if (_dateTimeProvider.UtcNow <= _lastUpdateTime.Add(UpdateInterval))
        {
            return;
        }

        try
        {
            var timeResult = await _crowGameClient.GetCurrentTime(cancellationToken);
            var utcNow = _dateTimeProvider.UtcNow;

            _serviceTimeOffset = timeResult.CurrentUtcTime - utcNow;
            _lastUpdateTime = utcNow;
        }
        catch (Exception ex)
        {
            _logger.LogError("An exception occurred while trying to update the service time {@Ex}", ex);
        }
    }
}
