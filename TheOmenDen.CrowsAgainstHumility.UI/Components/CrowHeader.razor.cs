using Blazorise;
using System.Reflection;

namespace TheOmenDen.CrowsAgainstHumility.Components;

public partial class CrowHeader: ComponentBase
{
    private Bar _topBar;

    private bool _topBarVisible = false;
    
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
