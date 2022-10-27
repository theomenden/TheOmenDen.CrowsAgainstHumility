using Blazorise;
using Microsoft.AspNetCore.Identity;
using TheOmenDen.CrowsAgainstHumility.Core.Auth.InputModels;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
#nullable disable
namespace TheOmenDen.CrowsAgainstHumility.Pages.Auth;

public partial class ForgotPassword: ComponentBase
{
    [Inject] NavigationManager NavigationManager { get; init; }

    [Inject] UserManager<ApplicationUser> UserManager { get; init; }

    [Inject] IEmailManagerService EmailManager { get; init; }

    [Inject]
    ILogger<ForgotPassword> Logger { get; init; }

    private Validations _validationsRef;

    private RecoveryInputModel Input { get; set; } = new ();

    private async Task OnResetClickedAsync()
    {
        try
        {
            var user = await UserManager.FindByEmailAsync(Input.Email);

            if (user is null || !(await UserManager.IsEmailConfirmedAsync(user)))
            {
                NavigationManager.NavigateTo("forgot-password-confirmation");
                return;
            }

            await EmailManager.BuildForgotPasswordEmailConfirmationAsync(user.Email, user, NavigationManager.BaseUri);

            NavigationManager.NavigateTo("forgot-password-confirmation");
        }
        catch (Exception ex)
        {
            Logger.LogError("Failed to reset password: @{Ex}", ex);
        }
    }
}
