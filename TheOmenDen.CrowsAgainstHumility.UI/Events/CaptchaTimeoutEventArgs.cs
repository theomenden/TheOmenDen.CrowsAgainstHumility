namespace TheOmenDen.CrowsAgainstHumility.Events;

public sealed class CaptchaTimeoutEventArgs: EventArgs
{
    public CaptchaTimeoutEventArgs(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }
    public string ErrorMessage { get; }
}
