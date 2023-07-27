using Azure.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TheOmenDen.CrowsAgainstHumility.Azure.Health;
using TheOmenDen.CrowsAgainstHumility.Azure.Repositories;

namespace TheOmenDen.CrowsAgainstHumility.Azure.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCorvidCosmosServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CosmosDbSettings>(options =>
        {
            options.ConnectionString = configuration["ConnectionStrings:omendencosmos"] ?? String.Empty;
            options.DatabaseName = configuration["Cosmos:DatabaseName"] ?? String.Empty;
            options.ContainerName = configuration["Cosmos:ContainerName"] ?? String.Empty;
        });

        // Use a Singleton instance of the CosmosClient
        services.AddSingleton<CosmosClient>(serviceProvider =>
        {
            var socketsHttpHandler = serviceProvider.GetRequiredService<SocketsHttpHandler>();
            var cosmosClientOptions = new CosmosClientOptions()
            {
                HttpClientFactory = () => new HttpClient(socketsHttpHandler, disposeHandler: false),
                EnableContentResponseOnWrite = false,
                ConnectionMode = ConnectionMode.Direct
            };

            return new(accountEndpoint: configuration["ConnectionStrings:acUri"], tokenCredential: new DefaultAzureCredential(), clientOptions: cosmosClientOptions);
        });

        services.AddSingleton<IServerStore, CosmosLobbyRepository>();

        services.AddHealthChecks()
            .AddCheck<CosmosDbHealthCheck>("CosmosDb Connection", tags: new[] { "Azure, Cosmos, NoSQL" })
            .AddCosmosDbCollection(configuration["ConnectionStrings:omenden-cosmosdb"], new DefaultAzureCredential(), database: "crowsagainsthumility",
                collections: new[] { "lobbies" },
                tags: new[] { "Azure", "Cosmos, NoSQL, Collections" });

        return services;
    }

    public static IServiceCollection AddCorvidRedisCaching(this IServiceCollection services, string connectionString)
    {
        services.AddHealthChecks()
            .AddCheck<RedisHealthCheck>("Redis", tags: new[] { "Azure", "Redis" });

        return services;
    }
}
