using Blazorise;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
using TheOmenDen.CrowsAgainstHumility.Identity.Extensions;

namespace TheOmenDen.CrowsAgainstHumility.Pages.Consumer;

public partial class Dashboard: ComponentBase
{
    [CascadingParameter] private Task<AuthenticationState> AuthenticationStateTask { get; set; }

    [Inject] private IDataProtectionProvider DataProtectionProvider { get; init; }

    [Inject] private NavigationManager NavigationManager { get; init; }

    [Inject] private UserManager<ApplicationUser> UserManager { get; init; }

    [Inject] private SignInManager<ApplicationUser> SignInManager { get; init; }

    [Inject] private IUserInformationService UserInformationService { get; init; }

    [Inject] private ILogger<Dashboard> Logger { get; init; }

    private Boolean HasErrors { get; set; }

    private ApplicationUser _user;

    private Modal _modalRef;

    private string _avatarImageSelectorUrl;

    private List<AuthenticationScheme> _externalProviders = new(5);

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        _user = await UserInformationService.GetCurrentUserAsync();

        _externalProviders = (await SignInManager.GetExternalAuthenticationSchemesAsync())
            .ToList();
    }

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
            Logger.LogError("Failed to login via {Provider}. Exception @{Ex}", authenticationScheme.DisplayName, ex);
        }
    }
}
