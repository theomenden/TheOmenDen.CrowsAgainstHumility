using Azure.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TheOmenDen.CrowsAgainstHumility.Azure.CosmosDb.Health;
using TheOmenDen.CrowsAgainstHumility.Azure.CosmosDb.Repositories;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Stores;
using TheOmenDen.CrowsAgainstHumility.Core.Models.Settings;

namespace TheOmenDen.CrowsAgainstHumility.Azure.CosmosDb.Extensions;
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

            return new (accountEndpoint: configuration["ConnectionStrings:acUri"], tokenCredential: new DefaultAzureCredential(), clientOptions: cosmosClientOptions);
        });
        
        services.AddSingleton<IServerStore, CosmosLobbyRepository>();

        services.AddHealthChecks()
            .AddCheck<CosmosDbHealthCheck>("CosmosDb Connection", tags: new[] { "Azure, Cosmos, NoSQL" })
            .AddCosmosDbCollection(configuration["ConnectionStrings:omenden-cosmosdb"], new DefaultAzureCredential(), database: "crowsagainsthumility",
                collections: new[] { "lobbies" },
                tags: new[] { "Azure", "Cosmos, NoSQL, Collections" });

        return services;
    }
}
