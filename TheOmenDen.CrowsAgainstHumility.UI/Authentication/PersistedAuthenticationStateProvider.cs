using System.Security.Claims;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace TheOmenDen.CrowsAgainstHumility.Authentication;

internal sealed class PersistedAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILogger<PersistedAuthenticationStateProvider> _logger;
    private readonly ISessionStorageService _sessionStorage;
    private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

    public PersistedAuthenticationStateProvider(ILogger<PersistedAuthenticationStateProvider> logger, ISessionStorageService sessionStorageService)
    {
        _logger = logger;
        _sessionStorage = sessionStorageService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var userSessionStorageResult = await _sessionStorage.GetItemAsync<ApplicationUserState>(nameof(ApplicationUserState));

            if (userSessionStorageResult is null)
            {
                _logger.LogInformation("Anonymous Authentication Claim");
                return await Task.FromResult(new AuthenticationState(_anonymous));
            }


            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new (ClaimTypes.Name, userSessionStorageResult.Username),
                new (ClaimTypes.Role, userSessionStorageResult.Role)
            }, "CustomAuth"));


            return await Task.FromResult(new AuthenticationState(claimsPrincipal));
        }
        catch (Exception ex)
        {
            _logger.LogError("An un-tracked state changed occurred - @{ex}", ex);
            return await Task.FromResult(new AuthenticationState(_anonymous));
        }
    }

    public async Task UpdateAuthenticationState(ApplicationUserState userSession)
    {
        ClaimsPrincipal claimsPrincipal;

        if (userSession is null)
        {
            await _sessionStorage.RemoveItemAsync("UserSession");
            claimsPrincipal = _anonymous;

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
            return;
        }

        await _sessionStorage.SetItemAsync("UserSession", userSession);
        claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new(ClaimTypes.Name, userSession.Username),
                new(ClaimTypes.Role, userSession.Role)
            }));


        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
    }
}
