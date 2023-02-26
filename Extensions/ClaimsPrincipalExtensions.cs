using System.Security.Claims;

namespace TheOmenDen.CrowsAgainstHumility.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        var val = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? String.Empty;

        return Guid.Parse(val);
    }

    public static Int64 GetLongUserId(this ClaimsPrincipal claimsPrincipal)
    {
        var val = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);

        return Int64.Parse(val);
    }

    public static (Boolean success, Guid userId) TryGetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.HasClaim(x => x.Type == ClaimTypes.NameIdentifier)
            ? (true, claimsPrincipal.GetUserId())
            : (false, Guid.Empty);
    }

    public static (Boolean success, Int64 userId) TryGetLongUserId(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.HasClaim(x => x.Type == ClaimTypes.NameIdentifier)
            ? (true, claimsPrincipal.GetLongUserId())
            : (false, -1);
    }
}
