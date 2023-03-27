namespace TheOmenDen.CrowsAgainstHumility.Data;
public sealed class TokenProvider
{
    /// <summary>
    /// Token provided by <see cref="Microsoft.AspNetCore.Antiforgery.IAntiforgery"/> to prevent CrossSite Scripting
    /// </summary>
    public String? XSRFToken { get; set; } = String.Empty;

    public String AccessToken { get; set; } = String.Empty;

    /// <summary>
    /// Token for refreshing <see cref="XSRFToken"/>
    /// </summary>
    public String? RefreshToken { get; set; } = String.Empty;

    public String Username { get; set; } = String.Empty;

    public Boolean IsAuthenticated { get; set; }
}
