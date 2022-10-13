using Microsoft.Extensions.DependencyInjection;
using TheOmenDen.CrowsAgainstHumility.Twitch.Interfaces;
using TheOmenDen.CrowsAgainstHumility.Twitch.Services;

namespace TheOmenDen.CrowsAgainstHumility.Twitch.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTwitchManagmentServices(this IServiceCollection services)
    {
        services.AddScoped<ITwitchRedemptionsManager, TwitchRedemptionsManagerService>();

        return services;
    }
}
