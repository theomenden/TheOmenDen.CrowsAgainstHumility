using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Cards;
using TheOmenDen.CrowsAgainstHumility.Core.DTO.ViewModels;

namespace TheOmenDen.CrowsAgainstHumility.Core.DTO.Models;
public sealed class CrowGameInputModel
{
    public string RoomName { get; set; } = String.Empty;

    public string GameCode { get; set; } = String.Empty;

    public int RoundTimeLimit { get; set; }

    public IEnumerable<PlayerViewModel> DesiredPlayers { get; set; } = Enumerable.Empty<PlayerViewModel>();

    public IEnumerable<Pack> DesiredPacks { get; set; } = Enumerable.Empty<Pack>();
}
