using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TheOmenDen.Shared.Guards;

namespace TheOmenDen.CrowsAgainstHumility.Azure.SignalR.Health;
internal class SignalRHealthCheck: IHealthCheck
{
    private readonly Func<HubConnection> _hubConnectionBuilder;

    public SignalRHealthCheck(Func<HubConnection> hubConnectionBuilder)
    {
        Guard.FromNull(hubConnectionBuilder, nameof(hubConnectionBuilder));
        _hubConnectionBuilder = hubConnectionBuilder;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        HubConnection? connection = null;

        try
        {
            connection = _hubConnectionBuilder();

            await connection.StartAsync(cancellationToken).ConfigureAwait(false);

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
        }
        finally
        {
            if (connection is not null)
            {
                await connection.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}
