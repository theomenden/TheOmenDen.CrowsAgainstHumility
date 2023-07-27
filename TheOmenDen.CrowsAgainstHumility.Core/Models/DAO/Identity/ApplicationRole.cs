using Microsoft.AspNetCore.Identity;

namespace TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Identity;
public sealed class ApplicationRole : IdentityRole<Guid>
{
    public ApplicationRole() {}

    public ApplicationRole(string roleName)
        : base(roleName) {}
}