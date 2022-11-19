using Blazorise;
using Discord.Interactions;
using Microsoft.AspNetCore.Identity;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
#nullable disable
namespace TheOmenDen.CrowsAgainstHumility.Pages.Auth;

public partial class Register: ComponentBase
{
    [Inject] private UserManager<ApplicationUser> UserManager { get; init; }
    
    [Inject] private IUserRegistrationService RegistrationService { get; init; }

    [Inject] private SignInManager<ApplicationUser> SignInManager { get; init; } 

    [Inject] private ILogger<Register> Logger { get; init; }
    
    [Inject] private IEmailManagerService EmailManager { get; init; }

    [Inject] private NavigationManager NavigationManager { get; init; }
    
    private Validations _validationsReference;

    private Core.Enumerations.NotificationType _selectedNotificationValue;

    protected RegisterInputModel InputModel { get; } = new();

    protected Boolean DoesHaveErrors { get; set; }

    protected IEnumerable<IdentityError> Errors { get; set; } = Enumerable.Empty<IdentityError>();

    private async Task OnRegisterClickedAsync()
    {
        try
        {
            DoesHaveErrors = false;
            Errors = Enumerable.Empty<IdentityError>();

            var user = new ApplicationUser
            {
                CreatedDate = DateTime.UtcNow,
                FirstName = InputModel.FirstName,
                LastName = InputModel.LastName,
                UserName = InputModel.Username,
                Email = InputModel.Email,
                NotificationType = _selectedNotificationValue ?? Core.Enumerations.NotificationType.Ignore
            };

            var createUserResult = await UserManager.CreateAsync(user, InputModel.Password);

            if (createUserResult.Succeeded)
            {
                await RegistrationService.AddDefaultRolesAsync();
                await RegistrationService.AddDefaultUserRoleAsync(user);

                switch (UserManager.Options.SignIn.RequireConfirmedEmail)
                {
                    case true:
                        await EmailManager.BuildRegistrationConfirmationEmailAsync(user.Email, user,
                            NavigationManager.BaseUri);

                        NavigationManager.NavigateTo("register-confirmation");
                        break;
                    case false:
                        await SignInManager.SignInAsync(user, isPersistent: false);

                        NavigationManager.NavigateTo("");
                        break;
                }

                return;
            }

            DoesHaveErrors = true;
            Errors = createUserResult.Errors;
        }
        catch (Exception ex)
        {
            DoesHaveErrors = true;
            Logger.LogError("Failed to register due to exception @{Ex}", ex);
        }
    }
}
