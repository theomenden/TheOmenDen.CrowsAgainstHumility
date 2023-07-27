#region Usings
using System.IO.Compression;
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Serilog;
using Serilog.Events;
using System.Net.Mime;
using TheOmenDen.Shared.Logging.Serilog;
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
using TheOmenDen.CrowsAgainstHumility.Identity.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using TheOmenDen.CrowsAgainstHumility.Areas.Models;
using Blazored.SessionStorage;
using System.Text.Json.Serialization;
using AspNet.Security.OAuth.Twitch;
using Azure.Storage.Blobs;
using Blazorise.FluentValidation;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Identity.Web;
using TheOmenDen.CrowsAgainstHumility.Identity.Utilities;
using TheOmenDen.CrowsAgainstHumility.Utilities;
using TheOmenDen.CrowsAgainstHumility;
using TheOmenDen.CrowsAgainstHumility.Azure.CosmosDb.Extensions;
using TheOmenDen.CrowsAgainstHumility.Azure.Extensions;
using TheOmenDen.CrowsAgainstHumility.Azure.Hubs;
using TheOmenDen.CrowsAgainstHumility.Azure.SignalR.Extensions;
using TheOmenDen.CrowsAgainstHumility.Azure.SignalR.Hubs;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Extensions;
using TheOmenDen.CrowsAgainstHumility.Core.Extensions;
using TheOmenDen.CrowsAgainstHumility.Core.Models.Settings;
using TheOmenDen.CrowsAgainstHumility.Core.Validators;
using TheOmenDen.CrowsAgainstHumility.Twitch.Extensions;
#endregion
#region Bootstrap Logger
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
    .Enrich.FromLogContext()
    .Enrich.WithThreadName()
    .Enrich.WithThreadId()
    .Enrich.WithProcessName()
    .Enrich.WithAssemblyVersion()
    .Enrich.WithAssemblyName()
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

    var vaultUri = builder.Configuration["VaultUri"] ?? String.Empty;

    builder.Configuration.AddAzureKeyVault(new Uri(vaultUri), new DefaultAzureCredential());

    builder.Host.UseDefaultServiceProvider(options => options.ValidateScopes = false)
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

    builder.Services.AddBlazorise(options =>
        {
            options.Immediate = true;
            options.IconStyle = IconStyle.DuoTone;
            options.LicenseKey = builder.Configuration["blazorise-commercial"] ?? String.Empty;
        })
        .AddBootstrap5Providers()
        .AddBootstrap5Components()
        .AddFontAwesomeIcons()
        .AddLoadingIndicator()
        .AddBlazoriseFluentValidation();

    builder.Services.AddApplicationInsightsTelemetry(options => options.ConnectionString = appInsightsConnectionString);

    var twitchKey = builder.Configuration["twitch-key"] ?? String.Empty;
    var twitchClient = builder.Configuration["twitch-clientId"] ?? String.Empty;

    var twitchStrings = new TwitchStrings(twitchKey, twitchClient);

    builder.Services.AddSingleton(twitchStrings);

    builder.Services
        .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = OpenIdConnectDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = TwitchAuthenticationDefaults.AuthenticationScheme;
        })
        .AddTwitter(options =>
        {
            var twitterKey = builder.Configuration["twitter-key"] ?? String.Empty;
            var twitterSecret = builder.Configuration["twitter-secret"] ?? String.Empty;
            var twitterBearer = builder.Configuration["twitter-bearer"] ?? String.Empty;

            var twitterKeys = new TwitterStrings(twitterKey, twitterSecret, twitterBearer);

            options.ConsumerKey = twitterKeys.Key;
            options.ConsumerSecret = twitterKeys.Secret;
            options.SaveTokens = true;
        })
        .AddTwitch(options =>
        {
            options.ClientId = twitchStrings.ClientId;
            options.ClientSecret = twitchStrings.Key;
            options.SaveTokens = true;
        })
        .AddDiscord(options =>
        {
            var discordClient = builder.Configuration["discord-clientId"] ?? String.Empty;
            var discordSecret = builder.Configuration["discord-secret"] ?? String.Empty;
            var discordKeys = new DiscordStrings(discordClient, discordSecret);

            options.ClientId = discordKeys.Id;
            options.ClientSecret = discordKeys.Secret;
            options.SaveTokens = true;
        })
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.Cookie.Name = builder.Configuration["Cookies:SharedCookieName"];
            options.Cookie.Path = builder.Configuration["Cookies:SharedCookiePath"];
            options.Cookie.HttpOnly = true;
            options.ExpireTimeSpan = TimeSpan.FromDays(5);

            options.LoginPath = "/Identity/Account/Login";
            options.AccessDeniedPath = "/Identity/Account/AccessDenied";

            options.SlidingExpiration = true;
        })
        .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"), cookieScheme: "microsoftCookies")
        .EnableTokenAcquisitionToCallDownstreamApi()
        .AddInMemoryTokenCaches();

    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.Cookie.Name = builder.Configuration["Cookies:SharedCookieName"];
        options.Cookie.Path = builder.Configuration["Cookies:SharedCookiePath"];
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(5);

        options.LoginPath = "/Identity/Account/Login";
        options.AccessDeniedPath = "/Identity/Account/AccessDenied";

        options.SlidingExpiration = true;
    });

    var socketsHttpHandler = new SocketsHttpHandler
    {
        PooledConnectionLifetime = TimeSpan.FromMinutes(5)
    };

    builder.Services.AddSingleton(socketsHttpHandler);

    builder.Services.AddHttpClient();
    builder.Services.AddScoped<ICookie, Cookie>();
    builder.Services.AddScoped<IClipboardService, ClipboardService>();
    builder.Services.AddScoped<TokenProvider>();
    builder.Services.AddScoped<TokenStateService>();
    var cacheConnectionString = builder.Configuration["CacheConnection"];

    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = cacheConnectionString ?? String.Empty;
    });

    var corvidConnectionString = builder.Configuration["ConnectionStrings:UserContextConnection"]
                                 ?? builder.Configuration["ConnectionStrings:CrowsAgainstHumilityDb"];

    builder.Services.AddCorvidDataServices(corvidConnectionString!);

    var connectionString = builder.Configuration["ConnectionStrings:CrowsAgainstAuthority"]
                           ?? builder.Configuration["ConnectionStrings:CrowsAgainstHumilityDb"];

    builder.Services.AddCorvidIdentityServices(connectionString!);

    builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
    {
        options.TokenLifespan = TimeSpan.FromDays(2);
    });

#if DEBUG
    builder.Services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(builder.Configuration["Cookies:PersistKeysDirectory"] ?? String.Empty))
            .SetApplicationName(builder.Configuration["Cookies:ApplicationName"] ?? String.Empty);
#endif

    if (builder.Environment.IsProduction())
    {
        var container = new BlobContainerClient(builder.Configuration["crowsagainststorage"],
            builder.Configuration["Application:KeyBase"]);
        const string blobName = "keys.xml";

        await container.CreateIfNotExistsAsync();

        var blobClient = container.GetBlobClient(blobName);

        var vaultKeyIdentifier = $"{vaultUri}{builder.Configuration["Application:KeyPathBase"]}";

        builder.Services.AddDataProtection()
            .PersistKeysToAzureBlobStorage(blobClient)
            .ProtectKeysWithAzureKeyVault(new Uri(vaultKeyIdentifier), new DefaultAzureCredential());
    }

    builder.Services.AddCorvidEmailServices(builder.Configuration["crowsagainstemails"] ?? String.Empty);

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

    var captchaSettings = new ReCaptchaSettings
    {
        CaptchaUri = builder.Configuration["Recaptcha:CaptchaUri"] ?? String.Empty,
        SiteKey = builder.Configuration["recaptcha-site"] ?? String.Empty,
        SecretKey = builder.Configuration["recaptcha-secret"] ?? String.Empty
    };

    builder.Services.AddSingleton(captchaSettings);

    builder.Services.AddTwitchManagementServices(builder.Configuration);

    builder.Services.AddSingleton<ISessionDetails, SessionDetails>();

    builder.Services.AddScoped<CircuitHandler, TrackingCircuitHandler>(sp => new TrackingCircuitHandler(sp.GetRequiredService<ISessionDetails>()));

    builder.Services.AddResponseCaching();

    builder.Services.AddSignalR(options => options.MaximumReceiveMessageSize = 104_857_600)
        .AddJsonProtocol();

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

    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
    builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<ApplicationUser>>();

    builder.Services.AddRazorPages();
    builder.Services.AddResponseCompression(options =>
        {
            options.MimeTypes = new[] { MediaTypeNames.Application.Octet };
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
        })
        .Configure<BrotliCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest)
        .Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.SmallestSize)
        .AddHsts(options =>
        {
            options.Preload = true;
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromDays(365);
        }); ;
    builder.Services.AddControllers();
    builder.Services.AddSingleton<HttpClient>();
    builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
    {
        options.TokenLifespan = TimeSpan.FromDays(2);
    });

    builder.Services.AddValidatorsFromAssemblies(new[]
        { typeof(App).Assembly, typeof(RegisterInputModelValidator).Assembly });

    builder.Services.AddScoped<IHtmlSanitizer, HtmlSanitizer>(options =>
    {
        var sanitizer = new HtmlSanitizer();
        sanitizer.AllowedAttributes.Add("class");

        return sanitizer;
    });

    builder.Services.AddCorvidProviders();
    builder.Services.AddCorvidCosmosServices(builder.Configuration);
    builder.Services.AddCorvidSignalServices(builder.Configuration["Azure:SignalR:ConnectionString"]);
    builder.Services.AddServerSideBlazor()
#if DEBUG
        .AddCircuitOptions(options => options.DetailedErrors = true)
#endif
        .AddHubOptions(options =>
        {
            options.MaximumReceiveMessageSize = 104_857_600;
        });
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddHttpClient<IRecaptchaService, ReCaptchaService>("captchaClient", cfg => cfg.BaseAddress = new Uri(builder.Configuration["Recaptcha:VerifyUri"] ?? String.Empty));
    builder.Services.AddScoped<IRecaptchaService, ReCaptchaService>();
    builder.Services.AddScoped<UserInfo>();
    builder.Services.AddScoped<IHostEnvironmentAuthenticationStateProvider>(sp =>
    {
        var provider = (ServerAuthenticationStateProvider)sp.GetRequiredService<AuthenticationStateProvider>();
        return provider;
    });

    await using var app = builder.Build();

    app.UseResponseCompression();
    // Configure the HTTP request pipeline.
    app.UseEnvironmentMiddleware(app.Environment);
    app.UseSerilogRequestLogging(options => options.EnrichDiagnosticContext = RequestLoggingConfigurer.EnrichFromRequest);

    app.UseWebSockets();
    app.UseHttpsRedirection();
    app.UsePathBase(app.Configuration["Application:PathBase"]);
    app.UseStaticFiles();

    app.UseRouting();

    app.UseApiExceptionHandler(options =>
    {
        options.AddResponseDetails = OptionsDelegates.UpdateApiErrorResponse;
        options.DetermineLogLevel = OptionsDelegates.DetermineLogLevel;
    });

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapGet("/robots.txt", SearchEngineGenerators.GenerateRobotsAsync);
    app.MapGet("/sitemap.txt", SearchEngineGenerators.GenerateSitemapAsync);
    app.MapGet("/sitemap.xml", SearchEngineGenerators.GenerateSitemapXmlAsync);

    app.UseMiddleware<CorvidSessionMiddleware>();

    app.MapHealthChecks("/healthcheck");

    app.MapControllers();
    app.MapRazorPages();
    app.MapHub<CrowGameHub>(CrowGameHub.HubUrl);
    app.MapBlazorHub();
    app.MapFallbackToPage("/_Host");

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal("An error occurred before {AppName} could launch: {@Ex}", nameof(TheOmenDen.CrowsAgainstHumility), ex);
}
finally
{
    await Log.CloseAndFlushAsync().ConfigureAwait(false);
}
