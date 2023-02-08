using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models;

public sealed record CawChatMessageDto(Guid Id, DateTime CreatedAt, ApplicationUser ToUser, ApplicationUser FromUser,
    String Message);
