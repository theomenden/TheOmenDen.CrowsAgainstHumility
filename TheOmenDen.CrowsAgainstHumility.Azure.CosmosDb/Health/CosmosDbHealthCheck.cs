using HealthChecks.CosmosDb;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TheOmenDen.Shared.Guards;

namespace TheOmenDen.CrowsAgainstHumility.Azure.CosmosDb.Health;
internal class CosmosDbHealthCheck: IHealthCheck
{
    private readonly CosmosClient _client;
    private readonly CosmosDbHealthCheckOptions _options;

    public CosmosDbHealthCheck(CosmosClient client, CosmosDbHealthCheckOptions options)
    {
        Guard.FromNull(client, nameof(client));
        Guard.FromNull(options, nameof(options));
        _client = client;
        _options = options;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            await _client.ReadAccountAsync().ConfigureAwait(false);

            if (_options.DatabaseId is not null)
            {
                var database = _client.GetDatabase(_options.DatabaseId);
                await database.ReadAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

                if (_options.ContainerIds?.Any() is true)
                {
                    foreach (var containerId in _options.ContainerIds)
                    {
                        await database
                            .GetContainer(containerId)
                            .ReadContainerAsync(cancellationToken: cancellationToken)
                            .ConfigureAwait(false);
                    }
                }
            }
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
        }
    }
}
