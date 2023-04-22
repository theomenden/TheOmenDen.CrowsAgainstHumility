using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.Constants;
using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Identity;

namespace TheOmenDen.CrowsAgainstHumility.Identity.Utilities;
public sealed class Registration
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ILogger<Registration> _logger;

    public Registration(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ILogger<Registration> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task SeedWithDefaultRolesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await AddRolesHelperAsync(ApplicationRoleNames.Administrator, cancellationToken);
            await AddRolesHelperAsync(ApplicationRoleNames.BetaUser, cancellationToken);
            await AddRolesHelperAsync(ApplicationRoleNames.AluSubscriber, cancellationToken);
            await AddRolesHelperAsync(ApplicationRoleNames.CorvidPatron, cancellationToken);
            await AddRolesHelperAsync(ApplicationRoleNames.GitHubSponsor, cancellationToken);
            await AddRolesHelperAsync(ApplicationRoleNames.StandardPlayer, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Exception occurred seeding with roles {@Ex}", ex);
        }
    }

    public async Task AddDefaultUserRoleAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        try
        {
            var users = await _userManager.Users.ToArrayAsync(cancellationToken);

            var administrators = await _userManager.GetUsersInRoleAsync(ApplicationRoleNames.Administrator);

            if (users.Any() && !administrators.Any())
            {
                await _userManager.AddToRoleAsync(user, ApplicationRoleNames.Administrator);
            }

            await _userManager.AddToRoleAsync(user, ApplicationRoleNames.StandardPlayer);
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
        params string[] roleNames)
        => await AddUserToRolesAsync(user, roleNames, cancellationToken);


    public async Task AddUserToRolesAsync(ApplicationUser user, IEnumerable<String> roleNames, CancellationToken cancellationToken = default)
    {
        try
        {
            var existingRoles = await _userManager.GetRolesAsync(user);

            await _userManager.RemoveFromRolesAsync(user, existingRoles);
            await _userManager.AddToRolesAsync(user, roleNames);
        }
        catch (Exception ex)
        {
            _logger.LogError("Exception: {@Ex}", ex);
        }
    }

    private async Task AddRolesHelperAsync(String roleName, CancellationToken cancellationToken = default)
    {
        var role = await _roleManager.FindByNameAsync(roleName);

        if (role is null)
        {
            await _roleManager.CreateAsync(new ApplicationRole(roleName));
        }
    }
}
