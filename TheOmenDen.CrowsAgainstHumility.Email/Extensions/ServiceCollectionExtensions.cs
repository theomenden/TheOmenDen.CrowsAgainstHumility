using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using SendGrid.Extensions.DependencyInjection;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
using TheOmenDen.CrowsAgainstHumility.Email.Services;

namespace TheOmenDen.CrowsAgainstHumility.Email.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCorvidEmailServices(this IServiceCollection services, string apiKey)
    {
        services.AddSendGrid(options =>
        {
            options.ApiKey = apiKey;
        });

        services.AddTransient<IEmailManagerService, EmailManagerService>();
        services.AddTransient<IEmailSender, EmailSenderService>();
        return services;
    }
}
