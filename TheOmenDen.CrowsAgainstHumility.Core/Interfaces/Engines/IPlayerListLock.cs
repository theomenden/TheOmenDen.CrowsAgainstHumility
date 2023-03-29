using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;

namespace TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Engines;
public interface IPlayerListLock: IDisposable
{
    PlayerList Players { get; }
    void Lock();
}
