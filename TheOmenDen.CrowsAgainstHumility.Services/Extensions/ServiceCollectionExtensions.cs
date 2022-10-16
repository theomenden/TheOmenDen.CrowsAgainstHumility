using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Discord;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Services.Authentication;
using TheOmenDen.CrowsAgainstHumility.Services.Interfaces;
using TwitchLib.Api;
using TwitchLib.Api.Core.Enums;

namespace TheOmenDen.CrowsAgainstHumility.Services.Extensions;
public static class ServiceCollectionExtensions
{
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
        var scopes = new List<TwitchLib.Api.Core.Enums.AuthScopes>()
        {
            AuthScopes.Channel_Read,
            AuthScopes.User_Read,
            AuthScopes.Helix_User_Read_Email
        };

        return new TwitchAPI()
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
