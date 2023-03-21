using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;

namespace TheOmenDen.CrowsAgainstHumility.Core.Requests;
public sealed class RegisterPlayerRequest
{
    public Player Player { get; set; }

    public CrowGame Game { get; set; }
}
