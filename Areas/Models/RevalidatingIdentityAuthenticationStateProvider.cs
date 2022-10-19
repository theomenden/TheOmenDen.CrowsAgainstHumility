using Discord.Commands;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace TheOmenDen.CrowsAgainstHumility.Areas.Models;

public sealed class RevalidatingIdentityAuthenticationStateProvider<TUser> :
    RevalidatingServerAuthenticationStateProvider where TUser : class
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IdentityOptions _identityOptions;

    public RevalidatingIdentityAuthenticationStateProvider(ILoggerFactory loggerFactory,
        IServiceScopeFactory serviceScopeFactory,
        IOptions<IdentityOptions> optionsAccessor)
    :base(loggerFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _identityOptions = optionsAccessor.Value;
    }

    protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);

    protected override async Task<Boolean> ValidateAuthenticationStateAsync(AuthenticationState authenticationState, CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        try
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<TUser>>();

            return await ValidateSecurityStampAsync(userManager, authenticationState.User, cancellationToken);
        }
        finally
        {
            if(scope is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync();
            }
            else
            {
                scope?.Dispose();
            }
        }
    }

    private async Task<bool> ValidateSecurityStampAsync(UserManager<TUser> userManager, ClaimsPrincipal principal, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            cancellationToken.ThrowIfCancellationRequested();
        }

        var user = await userManager.GetUserAsync(principal);

        if (user is null)
        {
            return false;
        }

        if (!userManager.SupportsUserSecurityStamp)
        {
            return true;
        }

        var principalStamp = principal.FindFirstValue(_identityOptions.ClaimsIdentity.SecurityStampClaimType);
        
        var userStamp = await userManager.GetSecurityStampAsync(user);

        return principalStamp == userStamp;
    }
}
