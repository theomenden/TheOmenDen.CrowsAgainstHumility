using System.Security.Principal;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;

namespace TheOmenDen.CrowsAgainstHumility.Components;

public partial class LoginDisplay : FluxorComponent
{
    [CascadingParameter] private Task<AuthenticationState> AuthenticationStateTask { get; set; }
    [Inject] private UserManager<ApplicationUser> UserManager { get; init; }
    [Inject] private IUserService UserService { get; init; }

    private AuthenticationState? _authState;
    private string? _userDisplayName;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        _authState = await AuthenticationStateTask.ConfigureAwait(false);

        var (success, userId) = _authState.User.TryGetUserId();

        if (success)
        {
            var contextUser = await UserService.GetUserViewModelAsync(userId);

            _userDisplayName = contextUser?.Logins.FirstOrDefault()?.LoginUsername;
        }

        _userDisplayName ??= _authState.User.Identity?.Name;
    }

    private Boolean IsAdminRole() => _authState?.User.IsInRole("Administrator") ?? false;
    
}
