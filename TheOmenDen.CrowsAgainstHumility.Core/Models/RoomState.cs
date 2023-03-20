using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models;
public class RoomState : IEquatable<RoomState>
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string RoomCode { get; set; }

    public string? Password { get; set; }

    public Guid GameId { get; set; }

    public virtual CrowGame Game { get; set; }

    public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; } = new HashSet<ApplicationUser>(10);
    #region Equals Overloads
    public bool Equals(RoomState? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }
        return Id.Equals(other.Id)
               && String.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase)
               && String.Equals(RoomCode, other.RoomCode, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        return obj is RoomState other && Equals(other);
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(Id);
        hashCode.Add(Name, StringComparer.OrdinalIgnoreCase);
        hashCode.Add(RoomCode, StringComparer.OrdinalIgnoreCase);
        return hashCode.ToHashCode();
    }
    #endregion
    #region Operator Overloads
    public static bool operator ==(RoomState? left, RoomState? right) => Equals(left, right);
    public static bool operator !=(RoomState? left, RoomState? right) => !Equals(left, right);
    #endregion
}
