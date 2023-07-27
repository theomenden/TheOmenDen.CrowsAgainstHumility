namespace TheOmenDen.CrowsAgainstHumility.Middleware;

public class CookieAuthenticationHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly CookieConfiguration _cookieConfiguration;

    public CookieAuthenticationHandler(IHttpContextAccessor httpContextAccessor, CookieConfiguration cookieConfiguration)
    {
        _httpContextAccessor = httpContextAccessor;
        _cookieConfiguration = cookieConfiguration;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var authToken = _httpContextAccessor.HttpContext?.Request.Cookies[_cookieConfiguration.CookieName];

        if (authToken is not null)
        {

            request.Headers.Add("Cookie", $"{_cookieConfiguration.CookieName}={authToken}");
        }

        return base.SendAsync(request, cancellationToken);
    }
}
