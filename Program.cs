using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.Bootstrap;
using Serilog;
using Serilog.Events;
using System.Net.Mime;
using TheOmenDen.CrowsAgainstHumility.Extensions;
using TheOmenDen.Shared.Logging.Serilog;
using Microsoft.EntityFrameworkCore;
using TheOmenDen.CrowsAgainstHumility.Areas.Identity.Data;
using System.Reflection;
using TheOmenDen.CrowsAgainstHumility.Hubs;
using Microsoft.AspNetCore.Components.Server.Circuits;
using TheOmenDen.CrowsAgainstHumility.Circuits;
using TheOmenDen.CrowsAgainstHumility.Middleware;
using Azure.Identity;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Azure;
using Fluxor;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
    .Enrich.FromLogContext()
    .Enrich.WithThreadId()
    .Enrich.WithProcessName()
    .Enrich.WithEnvironmentUserName()
    .Enrich.WithMemoryUsage()
    .Enrich.WithEventType()
    .WriteTo.Async(a =>
    {
        a.File("./logs/log-.txt", rollingInterval: RollingInterval.Day);
        a.Console();
    })
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host
        .ConfigureAppConfiguration((context, config) =>
            {
                config
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, true)
                    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true)
                    .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
                    .AddEnvironmentVariables()
                    .AddAzureKeyVault(
                    new Uri(builder.Configuration["VaultUri"]),
                    new DefaultAzureCredential());

            })
        .UseDefaultServiceProvider(options => options.ValidateScopes = false)
        .UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithEventType()
            );

    builder.Logging
        .ClearProviders()
        .AddSerilog(dispose: true);

    // Add services to the container.
    builder.Services.AddBlazorise(options => options.Immediate = true)
        .AddBootstrap5Providers()
        .AddBootstrap5Components()
        .AddBootstrapIcons();

    builder.Services.AddAuthentication(options =>
        {
            /* Authentication options */
        })
        .AddTwitter(options =>
        {
            var twitterKeys = new TwitterStrings(
                builder.Configuration["twitter-key"],
                builder.Configuration["twitter-secret"],
                builder.Configuration["twitter-bearer"]);

            options.ConsumerKey = twitterKeys.Key;
            options.ConsumerSecret = twitterKeys.Secret;
        })
        .AddTwitch(options =>
        {
            var twitchKeys = new TwitchStrings(
                builder.Configuration["twitch-key"],
                builder.Configuration["twitch-clientId"]);

            options.ClientId = twitchKeys.ClientId;
            options.ClientSecret = twitchKeys.Key;
        })
        .AddDiscord(options =>
        {
            var discordKeys = new DiscordStrings(
                builder.Configuration["discord-clientId"],
                builder.Configuration["discord-secret"]
                );

            options.ClientId = discordKeys.Id;
            options.ClientSecret = discordKeys.Secret;
        });

    builder.Services.AddHttpClient();
    builder.Services.AddScoped<TokenProvider>();

    builder.Services.AddResponseCompression(options =>
    {
        options.MimeTypes = new[] { MediaTypeNames.Application.Octet };
    });
    var connectionString = builder.Configuration.GetConnectionString("UserContextConnection") ?? throw new InvalidOperationException("Connection string 'UserContextConnection' not found.");

    var currentAssembly = typeof(Program).Assembly;

    builder.Services.AddFluxor(options => options
        .ScanAssemblies(currentAssembly)
#if DEBUG
        .UseReduxDevTools(rdt =>
        {
            rdt.Name = nameof(TheOmenDen.CrowsAgainstHumility);
            rdt.UseSystemTextJson(_ =>
                new ()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                }
            );
        })
#endif
        .UseRouting()
        .AddMiddleware<StoreLoggingMiddleware>());

    builder.Services.AddSingleton<SessionDetails>();

    builder.Services.AddScoped<CircuitHandler, TrackingCircuitHandler>(sp => new TrackingCircuitHandler(sp.GetRequiredService<SessionDetails>()));

    builder.Services.AddDbContext<UserContext>(options =>
        options.UseSqlite(connectionString));

    builder.Services.AddDefaultIdentity<CAHUser>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddEntityFrameworkStores<UserContext>();

    builder.Services.AddResponseCaching();

    builder.Services.AddSignalR(options => options.MaximumReceiveMessageSize = 104_857_600);

    builder.Services.AddRazorPages();
    builder.Services.AddServerSideBlazor()
        .AddHubOptions(options =>
        {
            options.MaximumReceiveMessageSize = 104_857_600;
        });

    await using var app = builder.Build();

    app.UseResponseCompression();

    // Configure the HTTP request pipeline.
    app.UseEnvironmentMiddleware(app.Environment);
    app.UseSerilogRequestLogging(options => options.EnrichDiagnosticContext = RequestLoggingConfigurer.EnrichFromRequest);

    app.UseWebSockets();
    app.UseHttpsRedirection();

    app.UseStaticFiles();

    app.UseRouting();

    app.UseApiExceptionHandler(options =>
    {
        options.AddResponseDetails = OptionsDelegates.UpdateApiErrorResponse;
        options.DetermineLogLevel = OptionsDelegates.DetermineLogLevel;
    });

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapHub<CawHub>(CawHub.HubUrl);
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapRazorPages();
        endpoints.MapBlazorHub();
        endpoints.MapFallbackToPage("/_Host");
    });

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal("An error occurred before {AppName} could launch: {@Ex}", nameof(TheOmenDen.CrowsAgainstHumility), ex);
}
finally
{
    Log.CloseAndFlush();
}
