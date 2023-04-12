using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Services.Messages;
public sealed class MemberMessage: Message
{
    public Player? Member { get; set; }
}
