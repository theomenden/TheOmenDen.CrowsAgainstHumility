using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Services.Hubs;

namespace TheOmenDen.CrowsAgainstHumility.Services.Rooms;
public sealed class PublicRoom: CrowGameRoom
{
    public PublicRoom(IHubContext<CrowGameHub> context, String roomName, CrowRoomSettings roomSettings, Func<CrowGameRoom, CancellationToken, Task> gameEnded)
        :base(context, roomName, roomSettings, gameEnded) {}
}
