using TheOmenDen.CrowsAgainstHumility.Core.DTO.Models.Cards;

namespace TheOmenDen.CrowsAgainstHumility.Core.DTO.Results.Service;
public sealed class ReconnectLobbyResult: LobbyResult
{
    public long LastMessageId { get; set; }
    public WhiteCardDto? SelectedWhiteCard { get; set; }
}
