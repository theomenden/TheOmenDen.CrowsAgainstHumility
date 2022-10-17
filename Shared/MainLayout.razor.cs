using Blazorise;
using System.Reflection;
#nullable disable
namespace TheOmenDen.CrowsAgainstHumility.Shared;

public partial class MainLayout: LayoutComponentBase
{
    private Bar _sideBar;

    private Bar _topBar;

    private bool _topBarVisible = false;

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
