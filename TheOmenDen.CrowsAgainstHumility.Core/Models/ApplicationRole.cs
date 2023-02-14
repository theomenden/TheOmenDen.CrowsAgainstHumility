using Microsoft.AspNetCore.Identity;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models;
public sealed class ApplicationRole: IdentityRole<Guid>
{
    public ApplicationRole()
    {
    }

    public ApplicationRole(String roleName)
    : base(roleName)
    {
    }
}
