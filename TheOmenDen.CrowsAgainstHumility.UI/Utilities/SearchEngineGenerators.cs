using System.Net.Mime;
using System.Reflection;
using System.Xml.Linq;
using TheOmenDen.CrowsAgainstHumility.Utilities.Sitemap;

namespace TheOmenDen.CrowsAgainstHumility.Utilities;

public static class SearchEngineGenerators
{
    private const string PagesNamespace = "TheOmenDen.CrowsAgainstHumility.Pages";
    public static Task GenerateRobotsAsync(HttpContext context, CancellationToken cancellationToken = default)
    {
        var baseUrl = GetBaseUrl(context);

        context.Response.ContentType = MediaTypeNames.Text.Plain;

        return Task.WhenAll(
                context.Response.WriteAsync("User-agent: *\n", cancellationToken),
                context.Response.WriteAsync("Disallow: \n\n", cancellationToken),
                context.Response.WriteAsync($"Sitemap: {baseUrl}/sitemap.txt", cancellationToken));
    }

    public static async Task GenerateSitemapAsync(HttpContext context, CancellationToken cancellationToken = default)
    {
        var pages = typeof(App).Assembly
            .ExportedTypes
            .Where(p => p.IsSubclassOf(typeof(ComponentBase))
                        && !String.IsNullOrWhiteSpace(p?.Namespace)
                        && p.Namespace.StartsWith(PagesNamespace));

        var baseUrl = GetBaseUrl(context);
        foreach (var routeAttribute in pages
                     .Where(pageType => pageType.CustomAttributes is not null)
                     .SelectMany(pageType => pageType.GetCustomAttributes<RouteAttribute>()))
        {
            await context.Response.WriteAsync($"{baseUrl}{routeAttribute.Template}\n", cancellationToken);
        }
    }

    public static Task GenerateSitemapXmlAsync(HttpContext context, CancellationToken cancellationToken = default)
    {
        var pages = typeof(App).Assembly
            .ExportedTypes
            .Where(p => p.IsSubclassOf(typeof(ComponentBase))
                        && !String.IsNullOrWhiteSpace(p?.Namespace)
                        && p.Namespace.StartsWith(PagesNamespace));

        var baseUrl = GetBaseUrl(context);

        var nodes = pages.Where(x => x.CustomAttributes is not null)
            .SelectMany(x => x.GetCustomAttributes<RouteAttribute>())
            .Select(x => new MapNode(null, DateTime.UtcNow, null, $"{baseUrl}{x.Template}"))
            .ToArray();

        var serializedXml = Sitemap.Sitemap.WriteSitemapToString(nodes);

        return context.Response.WriteAsync(serializedXml, cancellationToken);
    }

    private static string GetBaseUrl(HttpContext context)
        => $"{context.Request.Scheme}://{context.Request.Host.Value}{context.Request.PathBase.Value}";
}
