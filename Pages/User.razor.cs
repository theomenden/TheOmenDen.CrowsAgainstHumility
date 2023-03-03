using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using TheOmenDen.CrowsAgainstHumility.Core.Auth.ViewModels;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;

namespace TheOmenDen.CrowsAgainstHumility.Pages;

[Authorize]
public partial class User: ComponentBase
{
    [Parameter] public string UserName { get; set; }

    [Inject] private ILogger<User> Logger { get; init; }

    [Inject] private NavigationManager NavigationManager { get; init; }

    [Inject] private UserManager<ApplicationUser> UserManager { get; init; }

    [Inject] private IUserService UserService { get; init; }

    private UserViewModel _userViewModel;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        var user = await UserManager.FindByNameAsync(UserName);

        if (user is not null)
        {
            _userViewModel = await UserService.GetUserViewModelAsync(user.Id);
        }
    }
}