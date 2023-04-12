using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Locks;
using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;
using TheOmenDen.CrowsAgainstHumility.Core.Resources;

namespace TheOmenDen.CrowsAgainstHumility.Services.Locks;
internal sealed class PlayerListLock: IPlayerListLock
{
    private readonly object _lockObject;
    private bool _isLocked;

    public PlayerListLock(PlayerList players, object lockObject)
    {
        Players = players;
        _lockObject = lockObject;
    }

    public PlayerList Players { get; private set; }


    public void Lock()
    {
        if (_isLocked)
        {
            return;
        }

        Monitor.TryEnter(_lockObject, 10_000, ref _isLocked);
        if (!_isLocked)
        {
            throw new TimeoutException(Resources.PlayerListTimeout);
        }
    }

    public void Dispose()
    {
        if (_isLocked)
        {
            Monitor.Exit(_lockObject);
        }
    }

    public ValueTask DisposeAsync()
    {
        if (_isLocked)
        {
            Monitor.Exit(_lockObject);
        }

        return ValueTask.CompletedTask;
    }
}
