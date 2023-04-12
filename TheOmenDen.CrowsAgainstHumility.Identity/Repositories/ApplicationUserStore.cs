using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Identity;
using TheOmenDen.CrowsAgainstHumility.Core.DTO.Models.Identity;
using TheOmenDen.CrowsAgainstHumility.Identity.Contexts;

namespace TheOmenDen.CrowsAgainstHumility.Identity.Repositories;
public class ApplicationUserStore : UserStore<
ApplicationUser,
ApplicationRole,
AuthDbContext,
Guid,
IdentityUserClaim<Guid>,
IdentityUserRole<Guid>,
ApplicationUserLogin,
IdentityUserToken<Guid>,
IdentityRoleClaim<Guid>>
{
    public ApplicationUserStore(AuthDbContext context, IdentityErrorDescriber? describer = null)
        : base(context, describer)
    {
    }

    public override Task AddLoginAsync(ApplicationUser user, UserLoginInfo login,
        CancellationToken cancellationToken = new())
    {
        if (login is not ApplicationUserLoginInfo applicationUserLogin)
        {
            return base.AddLoginAsync(user, login, cancellationToken);
        }

        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(login);

        ApplicationUserLogins.Add(new()
        {
            LoginProvider = applicationUserLogin.LoginProvider,
            ProviderKey = applicationUserLogin.ProviderKey,
            UserId = user.Id,
            ProviderDisplayName = applicationUserLogin.ProviderDisplayName,
            ProviderEmail = applicationUserLogin.ProviderEmail
        });

        return Task.FromResult(false);

    }

    private DbSet<ApplicationUserLogin> ApplicationUserLogins => Context.Set<ApplicationUserLogin>();
}
