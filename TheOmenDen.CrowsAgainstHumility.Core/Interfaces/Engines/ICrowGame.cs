using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Engines.Locks;
using TheOmenDen.CrowsAgainstHumility.Core.Messages;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;

namespace TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Engines;
public interface ICrowGame
{
    IEnumerable<string> PlayerListNames { get; }
    IPlayerListLock CreatePlayerList(string playerListName, string cardTsarName, IList<Pack> deck);
    IPlayerListLock GetPlayerList(string playerListName);
    IPlayerListLock AttachPlayerList(PlayerList playerList);
    Task<IEnumerable<Message>> GetMessagesAsync(Observer observer, CancellationToken cancellationToken = default);
    void DisconnectInactiveObservers();
    void DisconnectInactiveObservers(TimeSpan inactivityTime);
}
