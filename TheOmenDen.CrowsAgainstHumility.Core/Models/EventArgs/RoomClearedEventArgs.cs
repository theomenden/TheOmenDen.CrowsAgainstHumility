using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models.EventArgs;
public class RoomClearedEventArgs: CrowGameEventArgs
{
    public RoomClearedEventArgs(Guid serverId) : base(serverId) {}
}
