using Blazorise;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using System;
using TheOmenDen.CrowsAgainstHumility.Core.Auth.InputModels;
using TheOmenDen.CrowsAgainstHumility.Extensions;
using TheOmenDen.CrowsAgainstHumility.Identity.Extensions;

namespace TheOmenDen.CrowsAgainstHumility.Pages.Auth;

public partial class Login: ComponentBase
{
    [Inject] private ILogger<Login> Logger { get; init; }

    [Inject] private UserManager<ApplicationUser> UserManager { get; init; }

    [Inject] private IDataProtectionProvider DataProtectionProvider { get; init; }

    [Inject] private NavigationManager NavigationManager { get; init; }

    [Inject] private TokenProvider TokenProvider { get; init; }

    private Validations _validationsRef;

    protected LoginInputModel Input { get; set; } = new ();

    protected Boolean HasErrors { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        var(couldRetrieveEmail,email) = NavigationManager.TryGetQueryString<String>("email");
        var (couldCheckForErrors, hasErrors) = NavigationManager.TryGetQueryString<bool>("hasErrors");
        if (couldRetrieveEmail)
        {
            Input.Email = email;
        }

        if (couldCheckForErrors)
        {
            HasErrors = hasErrors;
        }
    }

    private async Task OnLogin()
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
            Logger.LogError("Failed to log in due to exception @{Ex}", ex);
        }
    }
}
