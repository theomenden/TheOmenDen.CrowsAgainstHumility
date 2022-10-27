using Blazorise;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using TheOmenDen.CrowsAgainstHumility.Core.Auth.InputModels;
using TheOmenDen.CrowsAgainstHumility.Extensions;

namespace TheOmenDen.CrowsAgainstHumility.Pages.Auth;

public partial class ResetPassword: ComponentBase
{
    [Inject] UserManager<ApplicationUser> UserManager { get; init; }
    [Inject] ILogger<ResetPassword> Logger { get; init; }
    [Inject] NavigationManager NavigationManager { get; init; }
    private ResetPasswordInputModel Input { get; set; } = new();

    private Validations _validationsRef;

    protected Boolean HasErrors { get; set; }

    protected IEnumerable<IdentityError> Errors { get; set; } = Enumerable.Empty<IdentityError>();

    protected override async Task OnInitializedAsync()
    {
        await  base.OnInitializedAsync();

        var (couldGetEmail, email) = NavigationManager.TryGetQueryString<string>("email");
        var (couldGetToken, token) = NavigationManager.TryGetQueryString<string>("token");
        if (couldGetEmail && couldGetToken)
        {
            Input.Email = email;
            Input.Token = token;
        }
    }

    private async Task OnRegisterClickedAsync()
    {
        try
        {
            HasErrors = false;
            Errors = Enumerable.Empty<IdentityError>();

            if (await _validationsRef.ValidateAll())
            {
                var user = await UserManager.FindByEmailAsync(Input.Email);

                if (user is null)
                {
                    NavigationManager.NavigateTo("reset-password-confirmation");
                    return;
                }


                var result = await UserManager.ResetPasswordAsync(user, Input.Token, Input.Password);

                if (result.Succeeded)
                {
                    NavigationManager.NavigateTo("reset-password-confirmation");
                    return;
                }

                HasErrors = true;
                Errors = result.Errors;
            }
        }
        catch (Exception ex)
        {
            HasErrors = true;
            Logger.LogError("Failed to reset password @{Ex}", ex);
        }
    }
}
