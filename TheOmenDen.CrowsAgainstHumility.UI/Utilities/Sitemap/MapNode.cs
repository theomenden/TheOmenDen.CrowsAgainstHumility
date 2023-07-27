namespace TheOmenDen.CrowsAgainstHumility.Utilities.Sitemap;

public sealed record MapNode(ChangeFrequency? ChangeFrequency, DateTime? LastModifiedAt, double? Priority, String Url);
