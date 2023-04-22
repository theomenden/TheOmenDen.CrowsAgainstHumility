using TheOmenDen.CrowsAgainstHumility.Core.Engine.Game;
using TheOmenDen.CrowsAgainstHumility.Core.Providers;

namespace TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus;
public interface IAzureCrowGame: ICrowGame
{
    IObservable<LobbyMessage> ObservableMessages { get; }
    DateTimeProvider DateTimeProvider { get; }
    GuidProvider GuidProvider { get; }

    void SetLobbiesInitializingList(IEnumerable<string> lobbyNames);
    void InitializeLobby(CrowGameServer lobby);
    void EndInitialization();
}
