using System.ComponentModel.DataAnnotations.Schema;

namespace TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Cards;
public partial class Pack : IComparable<Pack>, IEquatable<Pack>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsOfficialPack { get; set; }

    [NotMapped]
    public string OfficialPack => IsOfficialPack ? "Official" : string.Empty;

    [NotMapped] public int WhiteCardsInPack => WhiteCards?.Count() ?? 0;

    [NotMapped] public int BlackCardsInPack => BlackCards?.Count() ?? 0;

    public virtual IEnumerable<BlackCard> BlackCards { get; set; } = Enumerable.Empty<BlackCard>();
    public virtual IEnumerable<WhiteCard> WhiteCards { get; set; } = Enumerable.Empty<WhiteCard>();

    public int CompareTo(Pack other)
        => string.CompareOrdinal(Name, other.Name);

    public bool Equals(Pack? other)
        => other is not null
           && Id == other.Id
           && Name == other.Name;

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

        return obj is Pack other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name);
    }
}
