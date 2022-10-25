namespace TheOmenDen.CrowsAgainstHumility;

public sealed class ApplicationUserState
{
    /// <summary>
    /// Token provided by <see cref="Microsoft.AspNetCore.Antiforgery.IAntiforgery"/> to prevent CrossSite Scripting
    /// </summary>
    public String? Xsrf { get; set; } = String.Empty;

    public String? AccessToken { get; set; } = String.Empty;

    /// <summary>
    /// Token for refreshing <see cref="Xsrf"/>
    /// </summary>
    public String? RefreshToken { get; set; } = String.Empty;

    public String Username { get; set; } = String.Empty;

    public String Role { get; set; } = String.Empty;

    public Boolean IsAuthenticated { get; set; }
}
