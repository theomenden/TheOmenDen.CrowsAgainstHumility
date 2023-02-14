using System.Net.Mime;
using System.Reflection;
using System.Xml.Linq;

namespace TheOmenDen.CrowsAgainstHumility.Utilities;

public static class SearchEngineGenerators
{
    public static async Task GenerateRobotsAsync(HttpContext context, CancellationToken cancellationToken = default)
    {
        var baseUrl = GetBaseUrl(context);

        context.Response.ContentType = MediaTypeNames.Text.Plain;

        await context.Response.WriteAsync("User-agent: *\n", cancellationToken);
        await context.Response.WriteAsync("Disallow: \n\n", cancellationToken);
        await context.Response.WriteAsync($"Sitemap: {baseUrl}/sitemap.txt", cancellationToken);
    }

    public static async Task GenerateSitemapAsync(HttpContext context, CancellationToken cancellationToken = default)
    {
        var pages = typeof(App).Assembly
            .ExportedTypes
            .Where(p => p.IsSubclassOf(typeof(ComponentBase))
                        && !String.IsNullOrWhiteSpace(p?.Namespace)
                        && p.Namespace.StartsWith("TheOmenDen.CrowsAgainstHumility.Pages"));

        var baseurl = GetBaseUrl(context);
        foreach (var routeAttribute in pages
                     .Where(pageType => pageType.CustomAttributes is not null)
                     .SelectMany(pageType => pageType.GetCustomAttributes<RouteAttribute>()))
        {
            await context.Response.WriteAsync($"{baseurl}{routeAttribute.Template}\n", cancellationToken);
        }
    }

    public static async Task GenerateSitemapXmlAsync(HttpContext context, CancellationToken cancellationToken = default)
    {
        var pages = typeof(App).Assembly
            .ExportedTypes
            .Where(p => p.IsSubclassOf(typeof(ComponentBase))
                        && !String.IsNullOrWhiteSpace(p?.Namespace)
                        && p.Namespace.StartsWith("TheOmenDen.CrowsAgainstHumility.Pages"));

        var baseUrl = GetBaseUrl(context);

        var urls = pages.Where(x => x.CustomAttributes is not null)
            .SelectMany(x => x.GetCustomAttributes<RouteAttribute>())
            .Select(x => $"{baseUrl}{x.Template}")
            .ToArray();

        var sitemap = new XElement("urlset",
            new XAttribute($"{XNamespace.Xmlns}x", "http://www.sitemaps.org/schemas/sitemap/0.9"),
            urls.Select(url => new XElement("url",
                new XElement("loc", url),
                new XElement("lastmod", DateTime.UtcNow.ToString("O")))));

        await context.Response.WriteAsync(sitemap.ToString(), cancellationToken);
    }

    private static string GetBaseUrl(HttpContext context)
        => $"{context.Request.Scheme}://{context.Request.Host.Value}{context.Request.PathBase.Value}";
}
