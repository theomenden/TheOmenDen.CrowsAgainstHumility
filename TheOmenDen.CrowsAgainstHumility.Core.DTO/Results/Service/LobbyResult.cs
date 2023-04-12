using TheOmenDen.CrowsAgainstHumility.Core.DTO.Models.Players;

namespace TheOmenDen.CrowsAgainstHumility.Core.DTO.Results.Service;
public class LobbyResult
{
    public Lobby? Lobby { get; set; }
    public Guid SessionId { get; set; }
}
