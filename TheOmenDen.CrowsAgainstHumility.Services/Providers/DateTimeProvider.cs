namespace TheOmenDen.CrowsAgainstHumility.Services.Providers;
public class DateTimeProvider
{
    public static DateTimeProvider Instance { get; } = new ();

    public virtual DateTime Now => DateTime.Now;

    public virtual DateTime UtcNow => DateTime.UtcNow;
}
