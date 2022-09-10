using TheOmenDen.CrowsAgainstHumility.Middleware;

namespace TheOmenDen.CrowsAgainstHumility.Extensions;
public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseEnvironmentMiddleware(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseResponseCaching();

        if (env.IsDevelopmentOrStaging())
        {
            app.UseDeveloperExceptionPage();

            return app;
        }

        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();

        return app;
    }

    public static IApplicationBuilder UseApiExceptionHandler(this IApplicationBuilder applicationBuilder)
    {
        var options = new ApiExceptionOptions();

        return applicationBuilder.UseMiddleware<ExceptionLogger>(options);
    }

    public static IApplicationBuilder UseApiExceptionHandler(this IApplicationBuilder builder, Action<ApiExceptionOptions> configureOptions)
    {
        var options = new ApiExceptionOptions();

        configureOptions(options);

        return builder.UseMiddleware<ExceptionLogger>(options);
    }
}