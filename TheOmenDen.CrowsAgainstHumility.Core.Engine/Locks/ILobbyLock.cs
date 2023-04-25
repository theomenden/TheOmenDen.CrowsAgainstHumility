using TheOmenDen.CrowsAgainstHumility.Core.Engine.Game;

namespace TheOmenDen.CrowsAgainstHumility.Core.Engine.Locks;
public interface ILobbyLock: IDisposable, IAsyncDisposable
{
    ValueTask<CrowGameServer> GetGameServer();
    ValueTask LockAsync(CancellationToken cancellationToken = default);
}
