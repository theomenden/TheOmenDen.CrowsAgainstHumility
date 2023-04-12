using Azure.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using TheOmenDen.CrowsAgainstHumility.Azure.CosmosDb.Health;

namespace TheOmenDen.CrowsAgainstHumility.Azure.CosmosDb.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCorvidCosmosServices(this IServiceCollection services, string connectionString)
    {
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

            return new (accountEndpoint: connectionString, tokenCredential: new DefaultAzureCredential(), clientOptions: cosmosClientOptions);
        });

        services.AddHealthChecks()
            .AddCheck<CosmosDbHealthCheck>("CosmosDb Connection", tags: new[] { "Azure, Cosmos, NoSQL" })
            .AddCosmosDbCollection(connectionString, new DefaultAzureCredential(), database: "crowsagainsthumility",
                collections: new[] { "lobbies" },
                tags: new[] { "Azure", "Cosmos, NoSQL, Collections" });

        return services;
    }
}
