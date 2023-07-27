using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Cards;
using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Identity;

namespace TheOmenDen.CrowsAgainstHumility.Core.DTO.ViewModels;

public sealed record CrowGameInputViewModel(
    String LobbyCode,
    String LobbyName,
    IEnumerable<ApplicationUser> Participants,
    IEnumerable<Pack> DesiredPacks);
