namespace TheOmenDen.CrowsAgainstHumility.Events;

public sealed class CaptchaSuccessEventArgs: EventArgs
{
    public CaptchaSuccessEventArgs(string captchaResponse)
    {
        CaptchaResponse = captchaResponse;  
    }
    public string CaptchaResponse { get; }
}
