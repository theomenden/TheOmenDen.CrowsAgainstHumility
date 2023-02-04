using Blazorise;
using System.Reflection;
using TheOmenDen.CrowsAgainstHumility.Core.Auth.ViewModels;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;

#nullable disable
namespace TheOmenDen.CrowsAgainstHumility.Shared;

public partial class MainLayout: LayoutComponentBase
{
    [Inject] private ILogger<MainLayout> Logger { get; init; }

    [Inject] private IUserInformationService UserInfo { get; set; }

    [Inject] private IUserService UserService { get; init; }

    private Bar _sideBar;

    private Bar _topBar;

    private bool _topBarVisible = false;
    
    private Guid CurrentUserId { get; set; }

    private UserViewModel UserViewModel { get; set; }

    protected override async Task OnInitializedAsync()
    { 
        await base.OnInitializedAsync();

        CurrentUserId = await UserInfo.GetUserIdAsync();

        if (CurrentUserId != Guid.Empty)
        {
            UserViewModel = await UserService.GetUserViewModelAsync(CurrentUserId);
        }
    }

    private static string ApplicationTitle
    {
        get
        {
            var attributes = Assembly.GetExecutingAssembly()
                .GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            return attributes.Length == 0
                ? String.Empty
                : ((AssemblyTitleAttribute)attributes[0]).Title;
        }
    }
}
