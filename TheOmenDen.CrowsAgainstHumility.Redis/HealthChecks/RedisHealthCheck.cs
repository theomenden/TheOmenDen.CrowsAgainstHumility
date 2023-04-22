using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Settings;

namespace TheOmenDen.CrowsAgainstHumility.Azure.Redis.HealthChecks;
public sealed class RedisHealthCheck : IHealthCheck, IDisposable, IAsyncDisposable
{
    private readonly IAzureCrowGameConfiguration _configuration;
    private readonly object _redisLock = new();
    private ConnectionMultiplexer? _redis;
    private bool _isDisposed;

    public RedisHealthCheck(IAzureCrowGameConfiguration configuration) => _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

    private string ConnectionString
    {
        get
        {
            var connectionString = _configuration.ServiceBusConnectionString!;

            if (connectionString.StartsWith("REDIS:", StringComparison.Ordinal))
            {
                connectionString = connectionString[6..];
            }
            return connectionString;
        }
    }


    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new())
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException(nameof(RedisHealthCheck));
        }

        try
        {
            var redis = await ConnectAsync();
            await redis.GetDatabase().PingAsync();
            return HealthCheckResult.Healthy("Redis Connection is healthy.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Redis Connection is unhealthy.", ex);
        }
    }

    private async Task<ConnectionMultiplexer> ConnectAsync()
    {
        if (_redis is not null)
        {
            return _redis;
        }

        var redis = await ConnectionMultiplexer.ConnectAsync(ConnectionString);

        var shouldKeepConnection = false;

        lock (_redisLock)
        {
            if (_redis is null)
            {
                _redis = redis;
                shouldKeepConnection = true;
            }
        }

        if (shouldKeepConnection)
        {
            return _redis;
        }

        await redis.CloseAsync();
        await redis.DisposeAsync();

        return _redis;
    }

    #region Disposal Methods
    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        lock (_redisLock)
        {
            if (_redis is not null)
            {
                _redis.Dispose();
                _redis = null;
            }
        }

        _isDisposed = true;
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if (_isDisposed || _redis is null)
        {
            return;
        }

        await _redis.DisposeAsync();
        _redis = null;

        _isDisposed = true;
        GC.SuppressFinalize(this);
    }
    #endregion
}
