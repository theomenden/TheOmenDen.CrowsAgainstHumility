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

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
    .Enrich.FromLogContext()
    .Enrich.WithThreadId()
    .Enrich.WithProcessName()
    .Enrich.WithEnvironmentUserName()
    .Enrich.WithMemoryUsage()
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
                    .AddEnvironmentVariables();
            })
        .UseDefaultServiceProvider(options => options.ValidateScopes = false)
        .UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services));

    builder.Logging
        .ClearProviders()
        .AddSerilog();

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
        });

    builder.Services.AddRazorPages();
    builder.Services.AddServerSideBlazor();

    builder.Services.AddResponseCompression(options =>
    {
        options.MimeTypes = new[] { MediaTypeNames.Application.Octet };
    });
    var connectionString = builder.Configuration.GetConnectionString("UserContextConnection") ?? throw new InvalidOperationException("Connection string 'UserContextConnection' not found.");

    builder.Services.AddDbContext<UserContext>(options =>
        options.UseSqlite(connectionString));

    builder.Services.AddDefaultIdentity<CAHUser>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddEntityFrameworkStores<UserContext>();
    builder.Services.AddResponseCaching();

    var app = builder.Build();

    app.UseResponseCompression();

    // Configure the HTTP request pipeline.
    app.UseEnvironmentMiddleware(app.Environment);
    app.UseSerilogRequestLogging(options => options.EnrichDiagnosticContext = RequestLoggingConfigurer.EnrichFromRequest);

    app.UseWebSockets();
    app.UseHttpsRedirection();

    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();
    app.MapBlazorHub();
    app.MapFallbackToPage("/_Host");

    app.Run();
}
catch(Exception ex)
{
    Log.Fatal("An error occurred before {AppName} could launch: {@Ex}", nameof(TheOmenDen.CrowsAgainstHumility), ex);
}
finally
{
    Log.CloseAndFlush();
}
