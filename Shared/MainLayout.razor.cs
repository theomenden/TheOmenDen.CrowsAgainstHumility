using Blazorise;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Reflection;
using TheOmenDen.CrowsAgainstHumility.Core.Auth.ViewModels;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;

#nullable disable
namespace TheOmenDen.CrowsAgainstHumility.Shared;

public partial class MainLayout: LayoutComponentBase
{
    [Inject] private ILogger<MainLayout> Logger { get; init; }

    [Inject] private UserManager<ApplicationUser> UserManager { get; init; }

    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; init; }

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

    private static string AssemblyProductVersion
    {
        get
        {
            var attributes = Assembly.GetExecutingAssembly()
                .GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);
            return attributes.Length == 0 ?
                String.Empty :
                ((AssemblyFileVersionAttribute)attributes[0]).Version;
        }
    }

    private static string ApplicationDevelopmentCompany
    {
        get
        {
            var attributes = Assembly.GetExecutingAssembly()
                .GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
            return attributes.Length == 0 ?
                String.Empty :
                ((AssemblyCompanyAttribute)attributes[0]).Company;
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
