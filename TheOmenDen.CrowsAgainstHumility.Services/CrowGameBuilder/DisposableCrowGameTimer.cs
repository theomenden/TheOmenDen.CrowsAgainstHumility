using System.Timers;
using Microsoft.Extensions.Logging;

namespace TheOmenDen.CrowsAgainstHumility.Services.CrowGameBuilder;
internal sealed class DisposableCrowGameTimer : IDisposable
{

    private readonly Action _action;
    private Func<Action, Task> _dispatcherDelegate;
    private System.Timers.Timer? _timer;

    public DisposableCrowGameTimer(Action action, Func<Action, Task> dispatcherDelegate, TimeSpan interval)
    {
        _action = action;
        _dispatcherDelegate = dispatcherDelegate;

        var timer = new System.Timers.Timer(interval.TotalMilliseconds);
        _timer = timer;
        timer.Elapsed += OnTimeElapsed;
        timer.Start();
    }

    private async void OnTimeElapsed(object? sender, ElapsedEventArgs e)
    {
        try
        {
            await _dispatcherDelegate(_action);
        }
        catch
        {
            // For Future Logging
        }
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
