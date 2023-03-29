using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Xml.Linq;

namespace TheOmenDen.CrowsAgainstHumility.Azure.Cosmos.HealthChecks;
internal sealed class AzureCosmosDbHealthCheck : IHealthCheck
{
    public static Func<PlayerListAccessContext, CancellationToken, Task<bool>> PerformCosmosHealthCheck() =>
        async (context, _) =>
        {
            try
            {
                await context.Database.GetCosmosClient().ReadAccountAsync().ConfigureAwait(false);
            }
            catch (HttpRequestException)
            {
                return false;
            }
            return true;
        };
}
