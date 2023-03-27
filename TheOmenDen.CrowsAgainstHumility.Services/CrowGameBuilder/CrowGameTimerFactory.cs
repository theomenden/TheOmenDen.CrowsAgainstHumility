using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheOmenDen.CrowsAgainstHumility.Services.CrowGameBuilder;
internal sealed class CrowGameTimerFactory
{
    private readonly TimeSpan _interval;
    private Func<Action, Task>? _dispatcherDelegate;

    public CrowGameTimerFactory(TimeSpan interval)
    {
        _interval = interval;   
    }

    public IDisposable StartTimer(Action? action)
    {
        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        return _dispatcherDelegate is not null
            ? new DisposableCrowGameTimer(action, _dispatcherDelegate, _interval)
            : throw new InvalidOperationException("Timer cannot be started with a configured dispatcher");
    }

    internal void SetDispatcherDelegate(Func<Action, Task>? dispatcherDelegate)
    => _dispatcherDelegate = dispatcherDelegate;
}
