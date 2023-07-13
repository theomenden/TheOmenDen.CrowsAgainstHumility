using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using TheOmenDen.CrowsAgainstHumility.Azure.SignalR.Health;
using TheOmenDen.CrowsAgainstHumility.Azure.SignalR.Hubs;

namespace TheOmenDen.CrowsAgainstHumility.Azure.SignalR.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection  AddCorvidSignalServices(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<ICrowGameHubBroadcaster, CrowGameHubBroadcaster>();

        services.AddSignalR(options =>
            {
#if DEBUG
                options.EnableDetailedErrors = true;
#endif
                options.KeepAliveInterval = TimeSpan.FromSeconds(30);
                options.MaximumReceiveMessageSize = 104_857_600;
            })
            .AddAzureSignalR(builder => builder.ConnectionString = connectionString)
            .AddJsonProtocol(builder => builder.PayloadSerializerOptions = JsonSerializerOptions.Default);

        services.AddHealthChecks()
            .AddCheck<SignalRHealthCheck>("SignalR Health", tags: new[] { "Azure", "SignalR" });
        return services;
    }
}
