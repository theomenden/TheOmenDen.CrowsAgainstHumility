#region Usings
using Blazorise;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using TheOmenDen.CrowsAgainstHumility.Core.Auth.InputModels;
using TheOmenDen.CrowsAgainstHumility.Extensions;
using TheOmenDen.CrowsAgainstHumility.Identity.Extensions;
using TheOmenDen.CrowsAgainstHumility.Services;
using TwitchLib.Api;
#endregion
namespace TheOmenDen.CrowsAgainstHumility.Pages.Auth;

public partial class Login : ComponentBase
{
    #region Parameters
    [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }
    [Parameter] public String ReturnUrl { get; set; }
    #endregion
    #region Injected Members
    [Inject] private TokenStateService TokenStateService { get; init; }

    [Inject] private ILogger<Login> Logger { get; init; }

    [Inject] private UserManager<ApplicationUser> UserManager { get; init; }

    [Inject] private IDataProtectionProvider DataProtectionProvider { get; init; }

    [Inject] private NavigationManager NavigationManager { get; init; }

    [Inject] private TokenProvider TokenProvider { get; init; }

    [Inject] private SignInManager<ApplicationUser> SignInManager { get; init; }

    [Inject] private TwitchStrings TwitchStrings { get; init; }

    [Inject] private TwitchAPI TwitchAPI { get; init; }
    #endregion
    #region Private fields
    private Validations _validationsRef;

    private List<AuthenticationScheme> _externalProviders = new(5);
    #endregion
    protected LoginInputModel Input { get; set; } = new();

    protected Boolean HasErrors { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        var (couldRetrieveEmail, email) = NavigationManager.TryGetQueryString<string>("email");
        var (couldCheckForErrors, hasErrors) = NavigationManager.TryGetQueryString<bool>("hasErrors");
        var (couldGetReturnUrl, returnUrl) = NavigationManager.TryGetQueryString<string>("ReturnUrl");
        if (couldRetrieveEmail)
        {
            Input.Email = email;
        }

        if (couldCheckForErrors)
        {
            HasErrors = hasErrors;
        }



        if (String.IsNullOrWhiteSpace(ReturnUrl) && couldGetReturnUrl)
        {
            ReturnUrl = returnUrl ?? String.Empty;
        }

        _externalProviders = (await SignInManager.GetExternalAuthenticationSchemesAsync())
            .ToList();
    }

    private async Task OnLoginAsync()
    {
        try
        {
            HasErrors = false;

            if (await _validationsRef.ValidateAll())
            {
                var user = await UserManager.FindByEmailAsync(Input.Email);

                if (user is null)
                {
                    HasErrors = true;
                    return;
                }

                Input.Token = await UserManager.GenerateUserTokenAsync(user, TokenOptions.DefaultProvider, "Login");

                var data = $"{Input.Email}|{Input.Password}|{Input.Token}|{Input.IsRemembered}";
                var protector = DataProtectionProvider.CreateProtectorForLogin();
                var protectedData = protector.Protect(data);

                NavigationManager.NavigateTo($"Account/LoginInternal?data={protectedData}", true);
            }
        }
        catch (Exception ex)
        {
            HasErrors = true;
            Logger.LogError("Failed to log in due to exception {@Ex}", ex);
        }
    }

    private async Task OnExternalLoginAsync(AuthenticationScheme authenticationScheme)
    {
        try
        {
            var authState = await AuthenticationStateTask;

            var user = await UserManager.GetUserAsync(authState.User);

            if (user is not null)
            {
                var token = await UserManager.GenerateUserTokenAsync(user, TokenOptions.DefaultProvider, "Login");

                var data = $"{user.Id}|{token}|{authenticationScheme.Name}";

                var protector = DataProtectionProvider.CreateProtectorForLogin();

                var protectedData = protector.Protect(data);

                NavigationManager.NavigateTo($"/Account/LoginExternalProvider?data={protectedData}", true);
            }
        }
        catch (Exception ex)
        {
            HasErrors = true;
            Logger.LogError("Failed to login via {Provider}. Exception @{Ex}", authenticationScheme.DisplayName, ex);
        }
    }
}
