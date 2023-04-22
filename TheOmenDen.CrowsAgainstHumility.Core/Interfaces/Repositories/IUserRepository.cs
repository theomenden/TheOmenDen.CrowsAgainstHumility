using TheOmenDen.CrowsAgainstHumility.Core.Auth.ViewModels;
using TheOmenDen.CrowsAgainstHumility.Core.Criteria;
using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Identity;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Results;

namespace TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Repositories;
public interface IUserRepository
{
    Task<ApplicationUser> GetUserAsync(Guid userId, CancellationToken cancellationToken = default);

    ValueTask<bool> IsExistingUserAsync(string username, CancellationToken cancellationToken = default);

    Task<SearchResult<UserViewModel>> GetUsersAsync(SearchUsersCriteria criteria, CancellationToken cancellationToken = default);

    ApplicationUser GetUser(Guid userId);

    Task<UserViewModel> GetUserViewModelAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<ApplicationUser?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);

    ValueTask<bool> IsInRoleAsync(Guid userId, CancellationToken cancellationToken = default, params string[] roles);

    ValueTask<bool> HasNotificationTypeSetAsync(Guid userId, NotificationType notificationType,
        CancellationToken cancellationToken = default);
}
