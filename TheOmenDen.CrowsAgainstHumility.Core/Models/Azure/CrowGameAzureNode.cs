using System.Text;
using Microsoft.Extensions.Logging;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models.Azure;
public class CrowGameAzureNode : IDisposable, IAsyncDisposable
{
    #region Constants
    private const string DeletedPlayerPrefix = "Deleted:";
    private static readonly byte[] DeletedPlayerPrefixAsBytes = Encoding.UTF8.GetBytes(DeletedPlayerPrefix);
    #endregion
    #region Private Members
    private readonly InitializationList _playersToInitialize = new();
    private readonly PlayerListSerializer _playerListSerializer = new();
    private readonly ILogger<CrowGameAzureNode> _logger;

    private IDisposable? _sendNodeMessageSubscription;
    private IDisposable? _serviceBusPlayerListMessageSubscription;
    private IDisposable? _serviceBusPlayerListCreatedSubscription;
    private IDisposable? _serviceBusPlayerListMessageSubscription;
    private IDisposable? _serviceBusPlayerListsMessageSubscription;
    private IDisposable? _serviceBusAppWidePlayerListsMessageSubscription;
    private IDisposable? _serviceBusInitializePlayerListMessageSubscription;

    private volatile string? _processingPlayerListName;
    #endregion
    #region Constructors

    public CrowGameAzureNode(IAzure)
    {
        
    }
    #endregion
    #region  Disposal Methods
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public ValueTask DisposeAsync()
    {
        _ = Stop();
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Stop().Wait();
        }
    }
    #endregion

}
