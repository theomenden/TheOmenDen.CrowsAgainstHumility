namespace TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;
public sealed class CrowGame : IEquatable<CrowGame>, IComparable<CrowGame>
{
    #region Game Properties
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    #endregion
    #region Navigation Properties
    public RoomState Room { get; set; }
    public ICollection<Guid> Packs { get; set; } = new HashSet<Guid>();
    #endregion
    #region Overrides
    public bool Equals(CrowGame? other)
        => other is not null &&
           Id == other.Id;

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

        return obj is CrowGame other && Equals(other);
    }

    public override int GetHashCode() => HashCode.Combine(Id, CreatedAt);

    public int CompareTo(CrowGame? other) => other is null ? 1 : other.CreatedAt.CompareTo(CreatedAt);
    #endregion
    #region Operator Overloads
    public static bool operator ==(CrowGame? lhs, CrowGame? rhs) => lhs?.Equals(rhs) ?? rhs is { };

    public static bool operator !=(CrowGame? lhs, CrowGame? rhs) => !(lhs == rhs);
    #endregion
}
