using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Core.Requests;
public sealed class RegisterPlayerRequest
{
    public Player Player { get; set; }

    public CrowGame Game { get; set; }
}
