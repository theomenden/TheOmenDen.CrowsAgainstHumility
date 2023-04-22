using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Identity;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models;

public sealed record CawChatMessageDto(Guid Id, DateTime CreatedAt, ApplicationUser ToUser, ApplicationUser FromUser, String Message);