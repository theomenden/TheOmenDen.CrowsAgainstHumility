namespace TheOmenDen.CrowsAgainstHumility.Core.Models.Settings;
public sealed class ReCaptchaSettings
{
    public string CaptchaUri { get; init; } = string.Empty;
    public string SiteKey { get; init; } = string.Empty;
    public string SecretKey { get; init; } = string.Empty;
}
