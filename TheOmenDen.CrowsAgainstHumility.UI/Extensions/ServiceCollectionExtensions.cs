using TheOmenDen.CrowsAgainstHumility.Services;

namespace TheOmenDen.CrowsAgainstHumility.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrontEndCorvids(this IServiceCollection services)
    {
        services.AddScoped<ICawOutService, CawOutService>();

        return services;
    }
}
