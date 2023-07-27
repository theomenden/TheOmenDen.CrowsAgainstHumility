namespace TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Cards;
public partial class FilteredBlackCardsByPack : IEquatable<FilteredBlackCardsByPack>
{
    #region Properties
    public Guid PackId { get; set; }
    public Guid Id { get; set; }
    public String Message { get; set; }
    public Int32 PickAnswersCount { get; set; }
    #endregion
    #region IEquatable Overloads
    public bool Equals(FilteredBlackCardsByPack? other)
        => other is not null &&
           PackId.Equals(other.PackId)
           && Id.Equals(other.Id);

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

        return obj is FilteredBlackCardsByPack other && Equals(other);
    }

    public override int GetHashCode()
    {
        var hashCode = HashCode.Combine(PackId, Id);
        return hashCode;
    }
    #endregion
    #region Operator Overloads
    public static bool operator ==(FilteredBlackCardsByPack? lhs, FilteredBlackCardsByPack? rhs)
        => lhs is null ? rhs is null : Equals(lhs, rhs);
    public static bool operator !=(FilteredBlackCardsByPack? lhs, FilteredBlackCardsByPack? rhs)
        => !(lhs == rhs);
    #endregion
}