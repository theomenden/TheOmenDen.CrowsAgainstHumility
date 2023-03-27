using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Repositories;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
using TheOmenDen.CrowsAgainstHumility.Identity.Contexts;
using TheOmenDen.CrowsAgainstHumility.Identity.Repositories;
using TheOmenDen.CrowsAgainstHumility.Identity.Services;

namespace TheOmenDen.CrowsAgainstHumility.Identity.Extensions;
public static class ServiceCollectionExtensions
{
    private static readonly ILoggerFactory LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
    {
        builder.AddJsonConsole();
        builder.AddConsole();
    });

    public static IServiceCollection AddCorvidIdentityServices(this IServiceCollection services, String connectionString)
    {
        services.AddDbContextFactory<AuthDbContext>(options =>
        {
            options.UseSqlServer(connectionString)
                .EnableServiceProviderCaching()
#if DEBUG
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
#endif
                .UseLoggerFactory(LoggerFactory);
        });

        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredUniqueChars = 1;

                options.User.RequireUniqueEmail = true;

                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<AuthDbContext>()
            .AddSignInManager();

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 3;

            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            options.User.RequireUniqueEmail = true;

            options.SignIn.RequireConfirmedEmail = true;
            options.SignIn.RequireConfirmedAccount = true;
        });

        services
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IUserInformationService, UserInformationService>()
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IUserService, UserService>()
            .AddScoped<IUserRegistrationService, UserRegistrationService>()
            .AddScoped<IEmailConfigurationRepository, EmailConfigurationRepository>()
            .AddScoped<IEmailConfigurationService, EmailConfigurationService>()
            .AddScoped<IRoomStateRepository,RoomStateRepository>()
            .AddScoped<IExternalAuthenticationService, ExternalAuthenticationService>();

        return services;
    }
}
