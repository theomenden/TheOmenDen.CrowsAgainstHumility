namespace TheOmenDen.CrowsAgainstHumility.Services.Providers;
public class TaskProvider
{
    public static TaskProvider Default { get; } = new();

    public virtual Task Delay(TimeSpan delay, CancellationToken cancellationToken)
    {
        return Task.Delay(delay, cancellationToken);
    }
}
