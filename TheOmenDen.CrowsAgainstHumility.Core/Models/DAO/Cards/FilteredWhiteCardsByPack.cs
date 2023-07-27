namespace TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Cards;
public partial class FilteredWhiteCardsByPack : IEquatable<FilteredWhiteCardsByPack>
{
    public Guid PackId { get; set; }

    public Guid Id { get; set; }

    public String CardText { get; set; }

    public bool Equals(FilteredWhiteCardsByPack? other)
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

        return obj is FilteredWhiteCardsByPack other && Equals(other);
    }

    public override int GetHashCode()
    {
        var hashCode = HashCode.Combine(PackId, Id);
        return hashCode;
    }

    public static bool operator ==(FilteredWhiteCardsByPack? left, FilteredWhiteCardsByPack? right)
        => left is null ? right is null : Equals(left, right);

    public static bool operator !=(FilteredWhiteCardsByPack? left, FilteredWhiteCardsByPack? right)
        => !Equals(left, right);
}
