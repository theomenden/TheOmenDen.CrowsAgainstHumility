using TheOmenDen.CrowsAgainstHumility.Twitch.Events;

namespace TheOmenDen.CrowsAgainstHumility.Twitch.Services;
internal sealed class BitsRedeemManagerService: IDisposable
{
    private Boolean _disposedValue;

    public BitsRedeemManagerService(Int32 initialBits = 0)
    {
        Bits = initialBits;

        BitsGoal = Int32.MaxValue;
    }

    public Int32 BitsGoal { get; set; }

    public Int32 Bits { get; private set; }

    public event EventHandler<BitsUpdatedEventArgs>? OnBitsUpdated;

    public void AddBits(Int32 bitsToAdd)
    {
        if (bitsToAdd <= 0)
        {
            return;
        }

        Bits += bitsToAdd;

        OnBitsUpdated?.Invoke(this, new BitsUpdatedEventArgs { BitsRedeemed = Bits });
    }

    public void ResetBits(Boolean shouldSubtractFromGoal)
    {
        Bits = shouldSubtractFromGoal
            ? Math.Max(0, Bits - BitsGoal)
            : 0;

        OnBitsUpdated?.Invoke(this, new BitsUpdatedEventArgs { BitsRedeemed = Bits });
    }

    private void Dispose(Boolean disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                OnBitsUpdated += null;
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
