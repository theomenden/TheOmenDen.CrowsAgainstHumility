using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
public interface IUserToPlayerProcessingService
{
    Task<Player?> GetPlayerByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Player?> GetPlayerByUsernameAsync(String username, CancellationToken cancellationToken = default);
}
