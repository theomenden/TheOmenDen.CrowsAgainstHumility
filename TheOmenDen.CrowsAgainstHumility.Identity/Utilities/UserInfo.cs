using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Identity;

namespace TheOmenDen.CrowsAgainstHumility.Identity.Utilities;
public sealed class UserInfo
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    private ApplicationUser? _currentUser;

    public UserInfo(UserManager<ApplicationUser> userManager, AuthenticationStateProvider authenticationStateProvider)
    {
        _authenticationStateProvider= authenticationStateProvider;
        _userManager= userManager;
    }

    public async Task<Guid> GetUserIdAsync(CancellationToken cancellationToken = default)
    {
        var user = await GetClaimsPrincipalAsync(cancellationToken);

        return Guid.TryParse(user?.FindFirstValue(ClaimTypes.NameIdentifier), out var result)
            ? result 
            : Guid.Empty;
    }

    public async Task<String> GetUserNameAsync(CancellationToken cancellationToken = default)
    {
        var user = await GetClaimsPrincipalAsync(cancellationToken);

        return user?.Identity?.Name ?? String.Empty;
    }

    public async Task<ClaimsPrincipal?> GetClaimsPrincipalAsync(CancellationToken cancellationToken = default)
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();

        var user = authState.User;

        return user.Identity is { IsAuthenticated: false } ? null : user;
    }

    public async Task<ApplicationUser?> GetCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        var principal = await GetClaimsPrincipalAsync(cancellationToken);

        if (principal is null)
        {
            return null;
        }

        _currentUser ??= await _userManager.GetUserAsync(principal);

        return _currentUser;

    }
}
