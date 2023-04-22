using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Identity;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;

namespace TheOmenDen.CrowsAgainstHumility.Identity.Services;

internal sealed class UserInformationService : IUserInformationService
{
    private readonly UserManager<ApplicationUser> _userManager;

    private readonly AuthenticationStateProvider _authStateProvider;

    public UserInformationService(UserManager<ApplicationUser> userManager, AuthenticationStateProvider authStateProvider)
    {
        _userManager = userManager;
        _authStateProvider = authStateProvider;
    }

    public async Task<Guid> GetUserIdAsync(CancellationToken cancellationToken = default)
    {
        var user = await GetCurrentUserAsync(cancellationToken);

        return user?.Id ?? Guid.Empty;
    }

    public async Task<String> GetUsername(CancellationToken cancellationToken = default)
    {
        var user = await GetCurrentUserAsync(cancellationToken);

        return user?.UserName ?? string.Empty;
    }

    public async Task<ApplicationUser?> GetCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return null;
        }

        var authState = await _authStateProvider.GetAuthenticationStateAsync();

        var user = authState.User;

        if (user.Identity is not { IsAuthenticated: true })
        {
            return null;
        }

        var currentUser = await _userManager.GetUserAsync(user);

        return currentUser;
    }
}
