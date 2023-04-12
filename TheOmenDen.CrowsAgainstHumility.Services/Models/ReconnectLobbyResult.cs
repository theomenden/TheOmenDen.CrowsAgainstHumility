using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Services.Models;
public class ReconnectLobbyResult: LobbyResult
{
    public long LastMessageId { get; set; }
    public WhiteCard? SelectedWhiteCard { get; set; }
}
