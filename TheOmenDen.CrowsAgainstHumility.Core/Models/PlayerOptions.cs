using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models;
public sealed record PlayerOptions
{
    public string? Username { get; init; }
    public GameRoles? Role { get; init; }
}
