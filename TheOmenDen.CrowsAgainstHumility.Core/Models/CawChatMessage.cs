using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Identity;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models;
public class CawChatMessage : IEqualityComparer<CawChatMessage>, IComparable<CawChatMessage>
{
    #region Properties
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid FromUserId { get; set; }
    public Guid ToUserId { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public virtual ApplicationUser FromUser { get; set; }
    public virtual ApplicationUser ToUser { get; set; }
    #endregion
    #region Method Overrides
    public bool Equals(CawChatMessage? x, CawChatMessage? y)
    => x is null
            ? y is null
            : y is not null
              && x.Id == y.Id;

    public override Boolean Equals(Object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        return obj is CawChatMessage other && Equals(this, other);
    }

    public override Int32 GetHashCode() => HashCode.Combine(Id, ToUserId, FromUserId);
    public int GetHashCode(CawChatMessage obj) => obj.GetHashCode();
    public int CompareTo(CawChatMessage? other) => CreatedAt.CompareTo(other?.CreatedAt);
    #endregion
    #region Operator Overloads
    public static bool operator ==(CawChatMessage? left, CawChatMessage? right)
    => left?.Equals(right) ?? right is null;

    public static bool operator !=(CawChatMessage left, CawChatMessage right)
    => !(left == right);

    public static bool operator <(CawChatMessage? left, CawChatMessage? right)
    => left is null ? right is { } : left.CompareTo(right) < 0;

    public static bool operator <=(CawChatMessage? left, CawChatMessage? right)
    => left is null || left.CompareTo(right) <= 0;

    public static bool operator >(CawChatMessage? left, CawChatMessage? right)
    => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(CawChatMessage? left, CawChatMessage? right)
    => left is null 
        ? right is null 
        : left.CompareTo(right) >= 0;
    #endregion
}
