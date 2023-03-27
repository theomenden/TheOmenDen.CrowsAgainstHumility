using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.Constants;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;

namespace TheOmenDen.CrowsAgainstHumility.Identity.Services;

internal sealed class UserRegistrationService : IUserRegistrationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ILogger<UserRegistrationService> _logger;

    public UserRegistrationService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ILogger<UserRegistrationService> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task SeedWithDefaultRolesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await AddRolesHelperAsync(ApplicationRoleNames.Administrator);
            await AddRolesHelperAsync(ApplicationRoleNames.BetaUser);
            await AddRolesHelperAsync(ApplicationRoleNames.AluSubscriber);
            await AddRolesHelperAsync(ApplicationRoleNames.CorvidPatron);
            await AddRolesHelperAsync(ApplicationRoleNames.GitHubSponsor);
            await AddRolesHelperAsync(ApplicationRoleNames.StandardPlayer);
        }
        catch (Exception ex)
        {
            _logger.LogError("Exception occurred seeding with roles {@Ex}", ex);
        }
    }

    public async Task AddDefaultUserRoleAsync(ApplicationUser applicationUser, CancellationToken cancellationToken = default)
    {
        try
        {
            var users = await _userManager.Users.ToArrayAsync(cancellationToken);

            var administrators = await _userManager.GetUsersInRoleAsync(ApplicationRoleNames.Administrator);

            if (users.Any() && !administrators.Any())
            {
                await _userManager.AddToRoleAsync(applicationUser, ApplicationRoleNames.Administrator);
            }

            await _userManager.AddToRoleAsync(applicationUser, ApplicationRoleNames.StandardPlayer);
        }
        catch (Exception ex)
        {
            _logger.LogError("Exception: {@Ex}", ex);
        }
    }

    public async Task AddUserToRoleAsync(ApplicationUser user, String roleName)
    {
        try
        {
            if (!await _userManager.IsInRoleAsync(user, roleName))
            {
                await _userManager.AddToRoleAsync(user, roleName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Exception: {@Ex}", ex);
        }
    }


    public async Task AddUserToRolesAsync(ApplicationUser user, CancellationToken cancellationToken = default,
        params string[] roles)
        => await AddUserToRolesAsync(user, roles, cancellationToken);


    public async Task AddUserToRolesAsync(ApplicationUser user, IEnumerable<String> roles, CancellationToken cancellationToken = default)
    {
        try
        {
            var existingRoles = await _userManager.GetRolesAsync(user);

            await _userManager.RemoveFromRolesAsync(user, existingRoles);
            await _userManager.AddToRolesAsync(user, roles);
        }
        catch (Exception ex)
        {
            _logger.LogError("Exception: {@Ex}", ex);
        }
    }

    private async Task AddRolesHelperAsync(String roleName)
    {
        var role = await _roleManager.FindByNameAsync(roleName);

        if (role is null)
        {
            await _roleManager.CreateAsync(new ApplicationRole(roleName));
        }
    }
}
