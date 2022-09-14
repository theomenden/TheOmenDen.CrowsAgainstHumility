using TheOmenDen.Shared.Interfaces.Models;

namespace TheOmenDen.CrowsAgainstHumility.Circuits;

/// <summary>
/// 
/// </summary>
/// <param name="Email"></param>
/// <param name="Id"></param>
/// <param name="IsAuthenticated"></param>
/// <param name="Key"></param>
/// <param name="Name"></param>
public sealed record SessionUser(String UserId, Boolean IsAuthenticated,  String AuthenticationType)
{
    public static readonly SessionUser DefaultUser = new(String.Empty, false, String.Empty);
}