using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Engines;
using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;
using TheOmenDen.CrowsAgainstHumility.Core.Providers;

namespace TheOmenDen.CrowsAgainstHumility.Azure.Interfaces;
public interface IAzureCrowGame: ICrowGame
{
    IObservable<PlayerListMessage> ObservableMessages { get; }
    DateTimeProvider DateTimeProvider { get; }
    GuidProvider GuidProvider { get; }
    void SetInitializingPlayerLists(IEnumerable<string> playerListNames);
    void InitializePlayerList(PlayerList playerList);
    void EndInitialization();
}
