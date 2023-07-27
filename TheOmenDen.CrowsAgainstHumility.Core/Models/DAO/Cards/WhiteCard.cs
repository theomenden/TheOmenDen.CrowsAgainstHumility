namespace TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Cards;
public partial class WhiteCard : IComparable<WhiteCard>, IEquatable<WhiteCard>
{
    public Guid Id { get; set; }
    public string CardText { get; set; }
    public Guid PackId { get; set; }

    public virtual Pack Pack { get; set; }
    public int CompareTo(WhiteCard other) => Id.CompareTo(other.Id);
    public override string ToString() => CardText ?? string.Empty;

    public bool Equals(WhiteCard? other)
        => other is not null
           && Id == other.Id
           || PackId == other.PackId
               && CardText.Equals(other.CardText, StringComparison.OrdinalIgnoreCase);

    public override bool Equals(object? obj)
        => obj is not null
           && (ReferenceEquals(this, obj)
               || obj is WhiteCard other
               && Equals(other));

    public override int GetHashCode() => HashCode.Combine(Id, CardText, PackId);

    public static bool operator ==(WhiteCard left, WhiteCard right) => left.Equals(right);

    public static bool operator !=(WhiteCard left, WhiteCard right) => !(left == right);
}