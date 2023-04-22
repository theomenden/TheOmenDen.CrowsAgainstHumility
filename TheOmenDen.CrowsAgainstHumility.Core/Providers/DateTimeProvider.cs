namespace TheOmenDen.CrowsAgainstHumility.Core.Providers;
public sealed class DateTimeProvider : IDateTimeProvider
{
    public static DateTimeProvider Default { get; } = new ();
    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;
}
