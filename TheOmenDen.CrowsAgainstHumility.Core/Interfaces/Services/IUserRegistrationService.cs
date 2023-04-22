using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Identity;
using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
public interface IUserRegistrationService
{
    Task SeedWithDefaultRolesAsync(CancellationToken cancellationToken = default);
    Task AddDefaultUserRoleAsync(ApplicationUser applicationUser, CancellationToken cancellationToken = default);

    Task AddUserToRolesAsync(ApplicationUser user, IEnumerable<string> roles,
        CancellationToken cancellationToken = default);

    Task AddUserToRolesAsync(ApplicationUser user, CancellationToken cancellationToken = default, params string[] roles);
}
