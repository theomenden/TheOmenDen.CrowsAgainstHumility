namespace TheOmenDen.CrowsAgainstHumility.Core.Providers;
public sealed class GuidProvider : IGuidProvider
{
    public static GuidProvider Default { get; } = new();
    public Guid NewGuid() => Guid.NewGuid();
}
