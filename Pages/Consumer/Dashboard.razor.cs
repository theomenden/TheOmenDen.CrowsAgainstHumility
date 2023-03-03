using System.Collections.Immutable;
using Blazorise;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using TheOmenDen.CrowsAgainstHumility.Core.Auth.ViewModels;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
using TheOmenDen.CrowsAgainstHumility.Identity.Extensions;

namespace TheOmenDen.CrowsAgainstHumility.Pages.Consumer;

public partial class Dashboard : ComponentBase
{
    [CascadingParameter] private Task<AuthenticationState> AuthenticationStateTask { get; set; }
    #region Injected Members
    [Inject] private IDataProtectionProvider DataProtectionProvider { get; init; }
    [Inject] private NavigationManager NavigationManager { get; init; }
    [Inject] private UserManager<ApplicationUser> UserManager { get; init; }
    [Inject] private SignInManager<ApplicationUser> SignInManager { get; init; }
    [Inject] private IUserService UserService { get; init; }
    [Inject] private IPlayerVerificationService PlayerVerificationService { get; init; }
    [Inject] private ILogger<Dashboard> Logger { get; init; }
    #endregion
    #region Fields
    private Boolean HasErrors { get; set; }
    private IEnumerable<LoginViewModel> _userNames = Enumerable.Empty<LoginViewModel>();
    private ApplicationUser _user;
    private Modal _modalRef;
    private string _avatarImageSelectorUrl;
    private string _userProfileImageUrl = String.Empty;
    private List<AuthenticationScheme> _externalProviders = new(5);
    private const string ClassListForIcons = @"fa-brands fa-{0} text-light fs-3 fw-semibold";
    #endregion
    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateTask;

        var (success, userId) = authState.User.TryGetUserId();

        if (success)
        {
            var userInfo = await UserService.GetUserViewModelAsync(userId);

            _userNames = userInfo.Logins.ToImmutableArray();

            var userTwitchName = _userNames
                .Where(u => u.LoginProvider.Equals("twitch", StringComparison.OrdinalIgnoreCase))
                .Select(u => u.LoginUsername)
                .First();

            _userProfileImageUrl = await PlayerVerificationService.GetProfileImageUrlAsync(userTwitchName);
        }

        _userProfileImageUrl ??= String.Empty;

        _externalProviders = (await SignInManager.GetExternalAuthenticationSchemesAsync())
            .ToList();
    }

    private static string GetLoginProviderIcon(String loginProviderName)
    {
        var result = loginProviderName switch
        {
            _ when String.Equals(loginProviderName, "twitch", StringComparison.OrdinalIgnoreCase) => loginProviderName,
            _ when String.Equals(loginProviderName, "twitter", StringComparison.OrdinalIgnoreCase) => loginProviderName,
            _ when String.Equals(loginProviderName, "discord", StringComparison.OrdinalIgnoreCase) => loginProviderName,
            _ => String.Empty
        };

        return String.Format(ClassListForIcons, result.ToLowerInvariant());
    }

    private static String GetExternalLoginUrl(AuthenticationScheme authenticationScheme)
        => $"Account/challenge/{authenticationScheme.Name}";

    private async Task ShowProfileImageEditorAsync()
    {
        await _modalRef.Show();
    }

    private async Task SaveProfileImageAsync()
    {
        _user.ImageUrl = _avatarImageSelectorUrl;

        await UserManager.UpdateAsync(_user);
        await _modalRef.Hide();
    }

    private async Task OnExternalLoginAsync(AuthenticationScheme authenticationScheme)
    {
        try
        {
            var token = await UserManager.GenerateUserTokenAsync(_user, TokenOptions.DefaultProvider, "Login");

            var data = $"{_user.Id}|{token}|{authenticationScheme.Name}";

            var protector = DataProtectionProvider.CreateProtectorForLogin();

            var protectedData = protector.Protect(data);

            NavigationManager.NavigateTo($"/Account/AssociateLogin?data={protectedData}", true);
        }
        catch (Exception ex)
        {
            HasErrors = true;
            Logger.LogError("Failed to login via {Provider}. Exception: {@Ex}", authenticationScheme.DisplayName, ex);
        }
    }
}
