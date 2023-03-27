namespace TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
public interface IServiceTimeProvider
{
    TimeSpan ServiceTimeOffset { get; }

    Task UpdateServiceTimeOffset(CancellationToken cancellationToken = default);
}
