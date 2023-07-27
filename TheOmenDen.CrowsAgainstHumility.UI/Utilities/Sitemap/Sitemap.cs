using System.Globalization;
using System.Xml.Linq;

namespace TheOmenDen.CrowsAgainstHumility.Utilities.Sitemap;

public static class Sitemap
{
    private static readonly XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";

    public static string WriteSitemapToString(IEnumerable<MapNode> nodes)
    {
        var rootElement = new XElement(xmlns + "urlset");

        foreach (var mapNode in nodes)
        {
            var urlElement = new XElement(xmlns + "url",
                new XElement(xmlns + "loc", mapNode.Url),
                ShouldNotIncludeLastAModifiedAt(mapNode) ? null : new XElement(xmlns + "lastmod", mapNode.LastModifiedAt?.ToString("O")),
                ShouldNotIncludeFrequency(mapNode) ? null : new XElement(xmlns + "changefreq", mapNode.ChangeFrequency.ToString().ToLowerInvariant()),
                ShouldNotIncludePriority(mapNode) ? null : new XElement(xmlns + "priority", mapNode.Priority?.ToString("F1", CultureInfo.InvariantCulture))
                );

            rootElement.Add(urlElement);
        }

        var xDoc = new XDocument(rootElement);
        return xDoc.ToString();
    }

    private static bool ShouldNotIncludeLastAModifiedAt(MapNode node) => !node.LastModifiedAt.HasValue;
    private static bool ShouldNotIncludeFrequency(MapNode node) => node.ChangeFrequency is null;
    private static bool ShouldNotIncludePriority(MapNode node) => !node.Priority.HasValue;
}
