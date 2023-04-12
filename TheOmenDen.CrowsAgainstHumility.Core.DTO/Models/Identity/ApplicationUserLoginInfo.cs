using Microsoft.AspNetCore.Identity;

namespace TheOmenDen.CrowsAgainstHumility.Core.DTO.Models.Identity;
public class ApplicationUserLoginInfo: UserLoginInfo
{
    public ApplicationUserLoginInfo(string loginProvider, string providerKey, string? displayName, string email) 
        : base(loginProvider, providerKey, displayName)
    {
        ProviderEmail = email;
    }

    public string ProviderEmail { get; set; }
}
