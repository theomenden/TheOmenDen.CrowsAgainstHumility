using TheOmenDen.CrowsAgainstHumility.Core.Auth.ViewModels;
using TheOmenDen.CrowsAgainstHumility.Core.Criteria;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Core.Results;

namespace TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
public interface IUserService
{
    Task<ApplicationUser> GetUserAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<SearchResult<UserViewModel>> GetUsersAsync(SearchUsersCriteria criteria,
        CancellationToken cancellationToken = default);

    ApplicationUser GetUser(Guid userId);

    ValueTask<UserViewModel> GetUserViewModelAsync(Guid userId, CancellationToken cancellationToken = default);

    ValueTask<ApplicationUser> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);

    ValueTask<bool> IsInRoleAsync(Guid userId, string role, CancellationToken cancellationToken = default);

    ValueTask<bool> IsInRoleAsync(Guid userId, IEnumerable<string> roles, CancellationToken cancellationToken = default);

    ValueTask<bool> IsInRoleAsync(Guid userId, CancellationToken cancellationToken = default, params string[] roles);

    ValueTask<bool> IsExistingUserAsync(string username, CancellationToken cancellationToken = default);
}
