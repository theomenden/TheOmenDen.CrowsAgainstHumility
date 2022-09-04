using System.Reflection;

namespace TheOmenDen.CrowsAgainstHumility.Shared;
public partial class CrowsAgainstHumilityFooter : ComponentBase
{
    private static string AssemblyProductVersion
    {
        get
        {
            var attributes = Assembly.GetExecutingAssembly()
                .GetCustomAttributes(typeof(AssemblyVersionAttribute), false);
            return attributes.Length == 0 ?
                String.Empty :
                ((AssemblyVersionAttribute)attributes[0]).Version;
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
