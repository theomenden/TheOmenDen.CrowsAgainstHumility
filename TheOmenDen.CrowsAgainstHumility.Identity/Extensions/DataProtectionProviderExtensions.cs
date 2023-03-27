using Microsoft.AspNetCore.DataProtection;

namespace TheOmenDen.CrowsAgainstHumility.Identity.Extensions;
public static class DataProtectionProviderExtensions
{
    private const string Login = nameof(Login);
    private const string Impersonation = nameof(Impersonation);

    public static IDataProtector CreateProtectorForLogin(this IDataProtectionProvider dataProtectionProvider)
    => dataProtectionProvider.CreateProtector(Login);

    public static IDataProtector CreateProtectorForImpersonation(this IDataProtectionProvider dataProtectionProvider)
        => dataProtectionProvider.CreateProtector(Impersonation);
}
