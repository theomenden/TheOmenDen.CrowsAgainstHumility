namespace TheOmenDen.CrowsAgainstHumility.Events;

public class CaptchaValidationErrorEventArgs: EventArgs
{
    public CaptchaValidationErrorEventArgs(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }
    public  string ErrorMessage { get; }
}
