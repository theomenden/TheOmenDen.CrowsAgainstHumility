namespace TheOmenDen.CrowsAgainstHumility.Services;

public sealed class TurnTimer: IDisposable
{
    public event EventHandler<Int32>? TurnTimerChanged;

    private readonly System.Timers.Timer _timer;

    internal TurnTimer()
    {
        _timer = new System.Timers.Timer()
        {
            Interval= 1000,
        };

        _timer.Elapsed += TimerElapsed;
    }
    
    public Int32 RemainingSeconds { get; private set; } = 0;

    internal void StartTimer(Int32 time)
    {
        RemainingSeconds = time;
        _timer.Start();
        TurnTimerChanged?.Invoke(this, RemainingSeconds);
    }

    internal void StopTimer()
    {
        _timer.Stop();
    }

    internal void SetTime(Int32 timeRemaining)
    {
        RemainingSeconds= timeRemaining;
    }

    private void TimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        RemainingSeconds -= 1;
        TurnTimerChanged?.Invoke(this, RemainingSeconds);

        if (RemainingSeconds <= 0)
        {
            _timer.Stop();
        }
    }

    public void Dispose()
    {
        _timer.Dispose();
    }
}
