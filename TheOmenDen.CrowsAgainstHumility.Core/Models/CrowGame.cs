using System.ComponentModel.DataAnnotations.Schema;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models;

public class CrowGame : IEquatable<CrowGame>
{
    public CrowGame()
    {
        Id = Guid.NewGuid();
        Name ??= String.Empty;
        LobbyCode ??= String.Empty;
    }
    #region Properties
    public Guid Id { get; set; }

    public Guid CrowGamePackId { get; set; }

    public Int32 CurrentPlayersInGame { get; set; }

    public virtual ICollection<CrowGamePlayer> Players { get; set; } = new HashSet<CrowGamePlayer>();

    public String Name { get; set; }

    public String LobbyCode { get; set; }

    public virtual ICollection<CrowGamePack> GameCardPacks { get; set; } = new HashSet<CrowGamePack>();
    #endregion
    #region Overrides
    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is CrowGame other && Equals(other);

    public bool Equals(CrowGame? other) =>
        other is not null && (ReferenceEquals(this, other) 
                              || Id.Equals(other.Id) &&
            string.Equals(LobbyCode, other.LobbyCode, StringComparison.OrdinalIgnoreCase));

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