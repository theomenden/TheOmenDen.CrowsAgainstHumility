using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Cards;
using TheOmenDen.CrowsAgainstHumility.Core.DTO.Models.Cards;

namespace TheOmenDen.CrowsAgainstHumility.Core.DTO.ViewModels.Game;

public sealed class CrowGameSessionViewModel
{
    public bool IsShown { get; set; }
    public bool CanShow { get; set; }
    public bool CanClear { get; set; }
    public bool CanPlayCards { get; set; }

    public IDictionary<string, WhiteCard>? PlayedCards { get; set; }
    public Deck PlayingDeck { get; set; }
}
