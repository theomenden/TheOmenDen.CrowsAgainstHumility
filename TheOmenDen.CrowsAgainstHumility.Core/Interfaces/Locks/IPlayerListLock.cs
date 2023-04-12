using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;

namespace TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Locks;
public interface IPlayerListLock: IDisposable, IAsyncDisposable
{
    PlayerList Players { get; }

    void Lock();
}
