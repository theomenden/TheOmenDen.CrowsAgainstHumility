using Microsoft.AspNetCore.SignalR;

namespace TheOmenDen.CrowsAgainstHumility.Pages
{
    public partial class GamesList: ComponentBase
    {
        private List<String> _currentGames = new(10);
    }
}
