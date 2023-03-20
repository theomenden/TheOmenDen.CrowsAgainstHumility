using Blazorise;
using System.Reflection;

namespace TheOmenDen.CrowsAgainstHumility.MobileComponents;

public partial class MobileCrowSider: ComponentBase
{
    private Bar _sideBar;

    [Inject] private NavigationManager NavigationManager { get; init; }

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
}
