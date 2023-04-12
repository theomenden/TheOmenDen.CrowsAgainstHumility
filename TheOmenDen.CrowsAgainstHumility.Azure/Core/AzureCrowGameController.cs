using System.Reactive.Subjects;
using TheOmenDen.CrowsAgainstHumility.Azure.Interfaces;
using TheOmenDen.CrowsAgainstHumility.Azure.Messages;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;

namespace TheOmenDen.CrowsAgainstHumility.Azure.Core;
public class AzureCrowGameController: CrowGameController, IAzureCrowGame, IInitializationStatusProvider, IDisposable, IAsyncDisposable
{
    private readonly Subject<PlayerListMessage> _observableMessages = new();
    private HashSet<string> _playerListsToInitialize;
    private object _playerListsToInitializeLock = new();
    private volatile bool _isInitialized;
}
