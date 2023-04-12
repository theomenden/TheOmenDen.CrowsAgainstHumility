using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Services.Models;
public class PlayerLobby
{
    public string Name { get; set; } = String.Empty;
    public LobbyMember? CardTsar { get; set; }
    public IEnumerable<LobbyMember> Members { get; set; } = Enumerable.Empty<LobbyMember>();
    public IEnumerable<LobbyMember> Observers { get; set; } = Enumerable.Empty<LobbyMember>();
    public LobbyState State { get; set; }
    public IEnumerable<WhiteCard> WhiteCards { get; set; } = Enumerable.Empty<WhiteCard>();
    public IEnumerable<PlayResultItem> RoundResult { get; set; } = Enumerable.Empty<PlayResultItem>();
    public DateTime? TimerEndTime { get; set; }
}
