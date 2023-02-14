using Microsoft.AspNetCore.Http;

namespace TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
public interface IExternalAuthenticationService
{
    Task<string> ExternalSignInAsync(HttpContext httpContext, CancellationToken cancellationToken = default);
}
