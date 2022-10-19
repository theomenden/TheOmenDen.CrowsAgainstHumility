using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
public interface IUserInformationService
{
    Task<Guid> GetUserIdAsync(CancellationToken cancellationToken = default);
    Task<string> GetUsername(CancellationToken cancellationToken = default);
    Task<ApplicationUser?> GetCurrentUserAsync(CancellationToken cancellationToken = default);
}