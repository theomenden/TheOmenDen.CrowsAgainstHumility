using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Services.Authentication;
using TwitchLib.Api;
using TwitchLib.Api.Core.Enums;
using TheOmenDen.CrowsAgainstHumility.Services.CardPoolBuilding;
using TheOmenDen.CrowsAgainstHumility.Services.CrowGameBuilder;

namespace TheOmenDen.CrowsAgainstHumility.Services.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCorvidGamesServices(this IServiceCollection services)
    {
        services.AddScoped<ICardPoolBuildingService, CardPoolBuildingService>()
            .AddScoped<ICrowGameService, CrowGameService>();
        return services;
    }

    public static IServiceCollection AddCorvidDiscordServices(this IServiceCollection services)
    {
        services.AddSingleton(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMembers,
        });

        services.AddSingleton<DiscordSocketClient>();

        return services;
    }

    public static IServiceCollection AddCorvidTwitchServices(this IServiceCollection services,
        TwitchStrings twitchStrings)
    {
        services.AddSingleton(InitializeTwitchApi(twitchStrings));

        services.AddScoped<IPlayerVerificationService, PlayerVerificationService>();

        return services;
    }

    private static TwitchAPI InitializeTwitchApi(TwitchStrings twitchStrings)
    {
        var scopes = new List<AuthScopes>()
        {
            AuthScopes.Channel_Read,
            AuthScopes.User_Read,
            AuthScopes.Helix_User_Read_Email
        };

        return new ()
        {
            Settings =
            {
                ClientId = twitchStrings.ClientId,
                Secret = twitchStrings.Key,
                Scopes = scopes,
                SkipAutoServerTokenGeneration = false
            }
        };
    }
}
