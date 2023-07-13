using Microsoft.AspNetCore.Identity;

namespace TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Identity;
public sealed class ApplicationUserLogin: IdentityUserLogin<Guid>
{
    public ApplicationUserLogin()
    {
    }

    public ApplicationUserLogin(Guid userId, string loginProvider, string providerKey, string providerDisplayName, string providerEmail)
    {
        UserId = userId;
        LoginProvider = loginProvider;
        ProviderKey = providerKey;
        ProviderDisplayName = providerDisplayName;
        ProviderEmail = providerEmail;
    }

    public string ProviderEmail { get; set; }
}
