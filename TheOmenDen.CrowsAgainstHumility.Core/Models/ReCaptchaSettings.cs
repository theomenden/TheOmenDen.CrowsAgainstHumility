namespace TheOmenDen.CrowsAgainstHumility.Core.Models;
public sealed class ReCaptchaSettings
{
    public String CaptchaUri { get; init; } = String.Empty;
    public String SiteKey { get; init; } = String.Empty;
    public String SecretKey { get; init; } = String.Empty;
}
