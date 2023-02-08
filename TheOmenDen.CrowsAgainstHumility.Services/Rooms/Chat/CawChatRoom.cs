using Microsoft.AspNetCore.SignalR;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Services.Hubs;

namespace TheOmenDen.CrowsAgainstHumility.Services.Rooms.Chat;
internal class CawChatRoom
{
    #region Private Fields
    private List<ApplicationUser> _chatters = new (10);
    private IHubContext<CawChatHub> _hubContext;
    #endregion

    public CawChatRoom()
    {
        
    }
}
