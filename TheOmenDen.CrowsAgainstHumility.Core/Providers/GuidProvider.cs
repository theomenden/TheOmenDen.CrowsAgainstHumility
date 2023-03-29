namespace TheOmenDen.CrowsAgainstHumility.Core.Providers;
public class GuidProvider
{
    public static GuidProvider Default { get; } = new();
    public virtual Guid NewGuid() => Guid.NewGuid();
}
