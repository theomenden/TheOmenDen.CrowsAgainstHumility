using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

public sealed class RedisHealthCheck : IHealthCheck, IDisposable
{
    private readonly IAzureCrowGameConfiguration _configuration;
    private readonly object _redisLock = new object();
    private ConnectionMultiplexer? _multiplexer;
    private bool _disposed;

    public RedisHealthCheck(IAzureCrowGameConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    private string ConnectionString
    {
        get
        {
            var connectionString = _configuration.RedisConnectionString;

            if (connectionString.StartsWith("REDIS:", StringComparison.OrdinalIgnoreCase))
            {
                connectionString = connectionString.Substring(6);
            }
            return connectionString ?? String.Empty;
        }
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(RedisHealthCheck));
        }

        try
        {
            var redis = await Connect();
            await redis.GetDatabase().PingAsync();
            return HealthCheckResult.Healthy("Redis connection is healthy.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Redis connection is unhealthy.", ex);
        }
    }

    private async Task<ConnectionMultiplexer> Connect()
    {
        if (_multiplexer is not null)
        {
            return _multiplexer;
        }

        var redis = await ConnectionMultiplexer.ConnectAsync(ConnectionString);

        var keepConnection = false;

        lock (_redisLock)
        {
            if (_multiplexer is null)
            {
                _multiplexer = redis;
                keepConnection = true;
            }
        }

        if (keepConnection)
        {
            return _multiplexer;
        }

        await redis.CloseAsync();
        await redis.DisposeAsync();

        return _multiplexer;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        lock (_redisLock)
        {
            if (_multiplexer is not null)
            {
                _multiplexer.Dispose();
                _multiplexer = null;
            }
        }
        _disposed = true;
    }
}