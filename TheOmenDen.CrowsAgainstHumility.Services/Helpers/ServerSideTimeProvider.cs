namespace TheOmenDen.CrowsAgainstHumility.Services.Helpers;
internal sealed class ServerSideTimeProvider
{
    public TimeSpan ServiceTimeOffset => TimeSpan.Zero;

    public Task UpdateServiceTimeOffset(CancellationToken cancellationToken = default) => Task.CompletedTask;
}
