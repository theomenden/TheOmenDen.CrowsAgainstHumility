using TheOmenDen.CrowsAgainstHumility.Azure.Core;

namespace TheOmenDen.CrowsAgainstHumility.Services;

public class AzureCrowGameNodeService: IHostedService, IDisposable, IAsyncDisposable
{
    private readonly CrowGameAzureNode _node;

    public AzureCrowGameNodeService(CrowGameAzureNode node)
    {
        _node = node ?? throw new ArgumentNullException(nameof(node));
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _ = _node.Start();
        return Task.CompletedTask;
    }
    public Task StopAsync(CancellationToken cancellationToken) => _node.Stop();
    public void Dispose()
    {
        _node.Dispose();
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await _node.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
