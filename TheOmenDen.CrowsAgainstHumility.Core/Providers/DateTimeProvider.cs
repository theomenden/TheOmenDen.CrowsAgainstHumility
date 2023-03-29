namespace TheOmenDen.CrowsAgainstHumility.Core.Providers;
public class DateTimeProvider
{
    public static DateTimeProvider Default { get; } = new ();
    public virtual DateTime Now => DateTime.Now;
    public virtual DateTime UtcNow => DateTime.UtcNow;
}
