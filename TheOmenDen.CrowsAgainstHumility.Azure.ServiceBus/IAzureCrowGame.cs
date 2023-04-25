using TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Messaging.Messages;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Game;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Stores;
using TheOmenDen.CrowsAgainstHumility.Core.Providers;

namespace TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus;
public interface IAzureCrowGame: IServerStore
{
    DateTimeProvider DateTimeProvider { get; }
    GuidProvider GuidProvider { get; }

    IObservable<LobbyMessage> GetObservableMessages();
    Task SetLobbiesInitializingListAsync(IEnumerable<string> lobbyNames, CancellationToken cancellationToken = default);
    Task InitializeLobbyAsync(CrowGameServer lobby, CancellationToken cancellationToken = default);
    Task EndInitializationAsync(CancellationToken cancellationToken = default);
}
