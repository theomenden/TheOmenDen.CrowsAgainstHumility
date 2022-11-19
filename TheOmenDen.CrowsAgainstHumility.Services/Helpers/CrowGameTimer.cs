using System.Timers;

namespace TheOmenDen.CrowsAgainstHumility.Services.Helpers;
internal sealed class CrowGameTimer: IDisposable
{
    private readonly System.Timers.Timer _timer;
    private DateTime _startTime;
    private DateTime _nextTrigger;

    public CrowGameTimer(double interval, ElapsedEventHandler handler)
    {
        _timer = new(interval)
        {
            AutoReset = false
        };

        _timer.Elapsed += handler;
    }

    public Double ElapsedTime => (DateTime.Now - _startTime).TotalMilliseconds;
    public Double RemainingTime => (_nextTrigger - DateTime.Now).TotalMilliseconds;

    public double ChangeTimerInterval(double multiplier)
    {
        _timer.Stop();

        var newInterval = multiplier * RemainingTime;

        _timer.Interval = newInterval;

        StartTimer();

        return RemainingTime;
    }

    public Double ResetTimer(double interval)
    {
        _timer.Stop();
        
        _timer.Interval = interval;
        
        StartTimer();

        return RemainingTime;
    }

    public void StartTimer()
    {
        _startTime = DateTime.Now;
        
        _nextTrigger = _startTime.Add(TimeSpan.FromMilliseconds(_timer.Interval));
        
        _timer.Start();
    }

    public void Dispose()
    {
        _timer.Stop();
        _timer.Dispose();
        GC.SuppressFinalize(this);
    }
}
