using TheOmenDen.CrowsAgainstHumility.Core.Auth.InputModels;
using TheOmenDen.CrowsAgainstHumility.Core.Auth.ViewModels;
using TheOmenDen.CrowsAgainstHumility.Core.Criteria;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Repositories;
using TheOmenDen.CrowsAgainstHumility.Core.Results;
using TheOmenDen.CrowsAgainstHumility.Identity.Contexts;

namespace TheOmenDen.CrowsAgainstHumility.Identity.Repositories;
internal sealed class UserRepository : IUserRepository
{
    private readonly IDbContextFactory<AuthDbContext> _dbContextFactory;

    public UserRepository(IDbContextFactory<AuthDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<ApplicationUser> GetUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Users.SingleAsync(p => p.Id == userId, cancellationToken);
    }

    public async ValueTask<bool> IsExistingUserAsync(string username, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Users.AnyAsync(p => p.UserName.Equals(username), cancellationToken);
    }

    public async Task<SearchResult<UserViewModel>> GetUsersAsync(SearchUsersCriteria criteria, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var normalizedRoles = criteria?.Roles?.Select(x => x.Normalize())?.ToArray() ?? Array.Empty<String>();

        var searchValue = criteria?.Value ?? String.Empty;

        var query = context.Users.AsQueryable();

        if (!String.IsNullOrWhiteSpace(searchValue))
        {
            query = query.Where(userInformation => userInformation.FirstName.Contains(searchValue)
                                                   && userInformation.LastName.Contains(searchValue)
                                                   && userInformation.UserName.Contains(searchValue)
                                                   && userInformation.Email.Contains(searchValue));
        }

        var viewModelQuery = from user in query
                             join userRole in context.UserRoles
                                     on user.Id equals userRole.UserId
                             join role in context.Roles
                                     on userRole.RoleId equals role.Id
                             where normalizedRoles.Contains(role.NormalizedName)
                             select new UserViewModel
                             {
                                 Id = user.Id,
                                 FirstName = user.FirstName,
                                 LastName = user.LastName,
                                 Username = user.UserName,
                                 Email = user.Email,
                                 IsEmailConfirmed = user.EmailConfirmed,
                                 RoleNames = new[] { role.Name },
                                 ImageUrl = user.GetUIImageUrl,
                             };

        var count = criteria?.Count is true
            ? await viewModelQuery.CountAsync(cancellationToken)
            : 0;

        var data = await viewModelQuery.ToArrayAsync(cancellationToken);

        return new(count, data);
    }

    public ApplicationUser GetUser(Guid userId)
    {
        using var context = _dbContextFactory.CreateDbContext();

        return context.Users.Single(p => p.Id == userId);
    }

    public async Task<UserViewModel> GetUserViewModelAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var userQuery = await (from user in context.Users
                               join userLogin in context.UserLogins
                                   on user.Id equals userLogin.UserId
                               where user.Id == userId
                               select new UserViewModel
                               {
                                   Id = user.Id,
                                   FirstName = user.FirstName,
                                   LastName = user.LastName,
                                   Username = user.UserName,
                                   Email = user.Email,
                                   IsEmailConfirmed = user.EmailConfirmed,
                                   ImageUrl = user.ImageUrl,
                                   Logins = new[] { new LoginViewModel(userLogin.LoginProvider, userLogin.ProviderDisplayName) }
                               }).FirstOrDefaultAsync(cancellationToken);
        
        return userQuery!;
    }

    public async Task<ApplicationUser?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var applicationUser = await (from user in context.Users
            join userLogin in context.UserLogins
                on user.Id equals userLogin.UserId
            where user.UserName == username
            select new ApplicationUser
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = userLogin.ProviderDisplayName ?? user.UserName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                ImageUrl = user.ImageUrl
            }).FirstOrDefaultAsync(cancellationToken);
        
        return applicationUser;
    }

    public async ValueTask<bool> IsInRoleAsync(Guid userId, CancellationToken cancellationToken = default, params string[] roles)
    {
        if (roles is null || !roles.Any())
        {
            return false;
        }

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var normalizedRoles = roles.Select(x => x.Normalize()).ToArray();

        var rolesQuery = await (from user in context.Users
                                join userRole in context.UserRoles
                                    on user.Id equals userRole.UserId
                                join role in context.Roles
                                    on userRole.RoleId equals role.Id
                                where user.Id == userId
                                      && normalizedRoles.Contains(role.NormalizedName)
                                select user.Id).AnyAsync(cancellationToken);

        return rolesQuery;
    }

    public async ValueTask<bool> HasNotificationTypeSetAsync(Guid userId, NotificationType notificationType,
        CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
        {
            return false;
        }

        user.NotificationType = notificationType;

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
