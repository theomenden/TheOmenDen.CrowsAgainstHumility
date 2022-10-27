#region Usings
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.Bootstrap;
using Serilog;
using Serilog.Events;
using System.Net.Mime;
using TheOmenDen.CrowsAgainstHumility.Extensions;
using TheOmenDen.Shared.Logging.Serilog;
using TheOmenDen.CrowsAgainstHumility.Hubs;
using Microsoft.AspNetCore.Components.Server.Circuits;
using TheOmenDen.CrowsAgainstHumility.Circuits;
using TheOmenDen.CrowsAgainstHumility.Middleware;
using Azure.Identity;
using System.Text.Json;
using Blazorise.LoadingIndicator;
using Fluxor;
using Ganss.Xss;
using TheOmenDen.CrowsAgainstHumility.Data.Extensions;
using Microsoft.Extensions.Logging.ApplicationInsights;
using TheOmenDen.CrowsAgainstHumility.Email.Extensions;
using TheOmenDen.CrowsAgainstHumility.Services;
using TheOmenDen.CrowsAgainstHumility.Services.Extensions;
using TheOmenDen.CrowsAgainstHumility.Identity.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using TheOmenDen.CrowsAgainstHumility.Areas.Models;
using Blazored.SessionStorage;
using System.Text.Json.Serialization;
using AspNet.Security.OAuth.Twitch;
using Microsoft.AspNetCore.Authentication.Cookies;
#endregion
#region Bootstrap Logger
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
#endregion
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

    var appInsightsConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];

    builder.Logging
        .ClearProviders()
        .AddApplicationInsights(
            config => config.ConnectionString = appInsightsConnectionString,
            options =>
            {
                options.FlushOnDispose = true;
                options.IncludeScopes = true;
                options.TrackExceptionsAsExceptionTelemetry = true;
            }
            )
        .AddFilter<ApplicationInsightsLoggerProvider>(typeof(Program).FullName, LogLevel.Trace)
        .AddSerilog(dispose: true);

    // Add services to the container.
    builder.Services.AddBlazorise(options => options.Immediate = true)
        .AddBootstrap5Providers()
        .AddBootstrap5Components()
        .AddBootstrapIcons()
        .AddLoadingIndicator();
    
    builder.Services.AddApplicationInsightsTelemetry(options => options.ConnectionString = appInsightsConnectionString);

    var twitchStrings = new TwitchStrings(builder.Configuration["twitch-key"], builder.Configuration["twitch-clientId"]);

    builder.Services.AddSingleton(twitchStrings);
    
    builder.Services
        .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme; 
            options.DefaultChallengeScheme    = TwitchAuthenticationDefaults.AuthenticationScheme;
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
            options.ClientId = twitchStrings.ClientId;
            options.ClientSecret = twitchStrings.Key;
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

    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(5);

        options.LoginPath = "/Identity/Account/Login";
        options.AccessDeniedPath = "/Identity/Account/AccessDenied";

        options.SlidingExpiration = true;
    });
    
    builder.Services.AddHttpClient();
    builder.Services.AddScoped<TokenProvider>();
    builder.Services.AddScoped<TokenStateService>();
    var cacheConnectionString = builder.Configuration["CacheConnection"];

    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = cacheConnectionString ?? String.Empty;
    });

    builder.Services.AddCorvidTwitchServices(twitchStrings);

    builder.Services.AddCorvidDiscordServices();

    var dbConnection = builder.Configuration["ConnectionStrings:UserContextConnection"]
                       ?? builder.Configuration["ConnectionStrings:CrowsAgainstHumilityDb"];

    builder.Services.AddCorvidDataServices(dbConnection);
    
    builder.Services.AddResponseCompression(options =>
    {
        options.MimeTypes = new[] { MediaTypeNames.Application.Octet };
    });

    var connectionString = builder.Configuration.GetConnectionString("CrowsAgainstAuthority") 
                           ?? throw new InvalidOperationException("Connection string 'UserContextConnection' not found.");

    builder.Services.AddCorvidIdentityServices(connectionString);

    var apiKey = builder.Configuration["crowsagainstemails"] ?? String.Empty;

    builder.Services.AddCorvidEmailServices(apiKey);
    
    var currentAssembly = typeof(Program).Assembly;

    builder.Services.AddFluxor(options => options
        .ScanAssemblies(currentAssembly)
#if DEBUG
        .UseReduxDevTools(rdt =>
        {
            rdt.Name = nameof(TheOmenDen.CrowsAgainstHumility);
            rdt.UseSystemTextJson(_ =>
                new()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                }
            );
        })
#endif
        .UseRouting()
        .AddMiddleware<StoreLoggingMiddleware>());

    builder.Services.AddCorvidGamesServices();

    builder.Services.AddSingleton<CrowGameService>();

    builder.Services.AddSingleton<ISessionDetails, SessionDetails>();

    builder.Services.AddScoped<CircuitHandler, TrackingCircuitHandler>(sp => new TrackingCircuitHandler(sp.GetRequiredService<ISessionDetails>()));
    
    builder.Services.AddResponseCaching();

    builder.Services.AddSignalR(options => options.MaximumReceiveMessageSize = 104_857_600)
        .AddMessagePackProtocol();

    builder.Services.AddBlazoredSessionStorage(options =>
    {
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.IgnoreReadOnlyProperties = true;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
        options.JsonSerializerOptions.WriteIndented = false;
    });
    builder.Services.AddRazorPages();
    builder.Services.AddControllers();
    builder.Services.AddSingleton<HttpClient>();
    builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
    {
        options.TokenLifespan = TimeSpan.FromDays(2);
    });

    builder.Services.AddDatabaseDeveloperPageExceptionFilter();

    builder.Services.AddScoped<IHtmlSanitizer, HtmlSanitizer>(options =>
    {
        var sanitizer = new HtmlSanitizer();
        sanitizer.AllowedAttributes.Add("class");

        return sanitizer;
    });

    builder.Services.AddServerSideBlazor()
#if DEBUG
        .AddCircuitOptions(options => options.DetailedErrors = true)
#endif
        .AddHubOptions(options =>
        {
            options.MaximumReceiveMessageSize = 104_857_600;
        });

    builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<ApplicationUser>>();
    
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
    
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
        endpoints.MapRazorPages();
        endpoints.MapBlazorHub();
        endpoints.MapHub<CawHub>(CawHub.HubUrl);
        endpoints.MapHub<CrowGameHub>(CrowGameHub.HubUrl);
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
