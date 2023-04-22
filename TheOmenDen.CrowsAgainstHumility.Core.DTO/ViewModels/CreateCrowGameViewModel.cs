using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Identity;
using TheOmenDen.CrowsAgainstHumility.Core.DTO.Models.Cards;

namespace TheOmenDen.CrowsAgainstHumility.Core.DTO.ViewModels;

public sealed record CreateCrowGameViewModel(string Name, string Code, IEnumerable<ApplicationUser> Players, Deck DesiredCards);
