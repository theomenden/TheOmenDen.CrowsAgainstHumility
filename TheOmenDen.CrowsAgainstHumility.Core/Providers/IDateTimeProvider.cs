namespace TheOmenDen.CrowsAgainstHumility.Core.Providers;

public interface IDateTimeProvider
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
}