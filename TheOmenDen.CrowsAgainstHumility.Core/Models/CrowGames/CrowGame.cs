namespace TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;
public class CrowGame: IEquatable<CrowGame>, IComparable<CrowGame>
{
    public Guid Id { get; set; }
    public Guid WhiteCardId { get; set; }
    public Guid BlackCardId { get; set; }
    public Guid RoomId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public virtual RoomState Room { get; set; }
    public virtual ICollection<WhiteCard> WhiteCards { get; init; } = new HashSet<WhiteCard>(1000);
    public virtual ICollection<BlackCard> BlackCards { get; init; } = new HashSet<BlackCard>(500);

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
}
