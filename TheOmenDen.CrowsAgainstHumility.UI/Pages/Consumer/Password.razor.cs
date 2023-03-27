using Blazorise;
using Microsoft.AspNetCore.Identity;
using TheOmenDen.CrowsAgainstHumility.Core.Auth.InputModels;
using TheOmenDen.CrowsAgainstHumility.Identity.Utilities;

namespace TheOmenDen.CrowsAgainstHumility.Pages.Consumer;

public partial class Password: ComponentBase
{
    #region Injected Members
    [Inject] private NavigationManager NavigationManager { get; init; }
    [Inject] private ILogger Logger { get; init; }
    [Inject] private IMessageService MessageService { get; init; }
    [Inject] private UserManager<ApplicationUser> UserManager { get; init; }
    [Inject] private SignInManager<ApplicationUser> SignInManager { get; init; }
    [Inject] private UserInfo UserInfo { get; set; }
    #endregion
    #region Private members
    private Validations _validationsRef;
    private ChangePasswordInputModel Input { get; set; } = new ();
    #endregion
    protected override async Task OnInitializedAsync() => await base.OnInitializedAsync();

    private async Task OnPasswordChangedAsync()
    {
        try
        {
            if (await _validationsRef.ValidateAll())
            {
                var userId = await UserInfo.GetUserIdAsync();
                var user =await UserManager.FindByIdAsync(userId.ToString());

                if (user is null)
                {
                    return;
                }

                var result = await UserManager.ChangePasswordAsync(user, Input.CurrentPassword, Input.NewPassword);

                if (!result.Succeeded)
                {
                    var errorMessage = String.Join(Environment.NewLine,
                        result?.Errors?.Select(error => error.Description)) 
                                       ?? String.Empty;

                    await MessageService.Error(errorMessage);

                    return;
                }

                Logger.LogInformation("User changed their password successfully.");

                await MessageService.Success(
                    "your password has been changed and you will automatically be signed-in with your new credentials",
                    "Password Updated");

                await SignInManager.RefreshSignInAsync(user);

                Input = new();
            }
        }
        catch (Exception ex)
        {
            Logger.LogError("Failed to reset password, exception: {@ex}", ex);
        }
    }

}
