using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Core.Requests;
public sealed class RegisterPlayerRequest
{
    public Player Player { get; set; }

    public CrowGame Game { get; set; }
}
