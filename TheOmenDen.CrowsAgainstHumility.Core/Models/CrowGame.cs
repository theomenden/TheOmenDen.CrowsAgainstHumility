namespace TheOmenDen.CrowsAgainstHumility.Core.Models;

public sealed class CrowGame : IEquatable<CrowGame>
{
    public CrowGame()
    {
        UsedBlackCards ??= new(70);
        UsedWhiteCards ??= new(70);
        Id = Guid.NewGuid();
        Name ??= String.Empty;
        Players ??= Enumerable.Empty<Player>();
        LobbyCode ??= String.Empty;
    }
    #region Properties
    public Guid Id { get; }

    public IEnumerable<Player> Players { get; set; }

    public Dictionary<Guid, BlackCard> UsedBlackCards { get; set; }

    public Dictionary<Guid, WhiteCard> UsedWhiteCards { get; set; }

    public Pack[] UsedPacks { get; set; }

    public String Name { get; set; }

    public String LobbyCode { get; set; }
    #endregion
    #region Card Pool Methods
    public void AddWhiteCardToUsedPool(WhiteCard cardToAdd)
    {
        UsedWhiteCards.TryAdd(cardToAdd.Id, cardToAdd);
    }

    public void AddBlackCardToUsedPool(BlackCard cardToAdd)
    {
        UsedBlackCards.TryAdd(cardToAdd.Id, cardToAdd);
    }

    public void ReleaseHeldWhiteCards(Int32 releaseThreshold)
    {
        if (UsedWhiteCards.Count < releaseThreshold)
        {
            return;
        }

        UsedWhiteCards.Clear();
    }

    public void ReleaseHeldBlackCards(Int32 releaseThreshold)
    {

        if (UsedBlackCards.Count < releaseThreshold)
        {
            return;
        }

        UsedBlackCards.Clear();
    }
    #endregion
    #region Overrides
    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is CrowGame other && Equals(other);

    public bool Equals(CrowGame? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Id.Equals(other.Id) && string.Equals(LobbyCode, other.LobbyCode, StringComparison.OrdinalIgnoreCase);
    }

    public override string ToString() => $"{Name}-{LobbyCode}";

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(Id);
        hashCode.Add(LobbyCode, StringComparer.OrdinalIgnoreCase);
        return hashCode.ToHashCode();
    }

    #endregion
    #region Operators
    public static bool operator ==(CrowGame? left, CrowGame? right) => Equals(left, right);

    public static bool operator !=(CrowGame? left, CrowGame? right) => !Equals(left, right);
    #endregion
}