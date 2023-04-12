using TheOmenDen.CrowsAgainstHumility.Core.DTO.Models.Cards;
using TheOmenDen.CrowsAgainstHumility.Core.DTO.Models.Service;

namespace TheOmenDen.CrowsAgainstHumility.Core.DTO.Results.Service;
public sealed class RoundResultItem
{
    public LobbyMember Member { get; set; }
    public WhiteCardDto? WhiteCard { get; set; }
}
