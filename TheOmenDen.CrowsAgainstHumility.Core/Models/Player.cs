namespace TheOmenDen.CrowsAgainstHumility.Core.Models;

public sealed class Player : IEquatable<Player>
{
    private const int MaxHandSize = 10;

    public Player() {}
    public Player(String connectionId)
    {
        ConnectionId = connectionId;
        Id = Guid.NewGuid();
    }

    public PlayerDto ToPlayerDto() => new(Name, Id, IsConnected);

    public String ConnectionId { get; set; }

    public int AwesomePoints { get; set; } = 2;

    public string Name { get; set; } = String.Empty;

    public string Username { get; set; } = String.Empty;

    public List<WhiteCard> Hand { get; } = new (MaxHandSize);

    public Guid ConnectionGuid { get; }

    public Guid Id { get; set;}

    public bool IsConnected { get; set; }

    public bool IsCardCzar { get; set; } = false;

    public bool Equals(Player? other) => other is not null 
                                         && (ReferenceEquals(this, other) 
                                             || ConnectionId == other.ConnectionId);

    public override bool Equals(object? obj) => obj is not null
        && ReferenceEquals(this, obj) 
        || obj is Player other 
        && Equals(other);

    public override int GetHashCode() => ConnectionId.GetHashCode();

    public static bool operator ==(Player? left, Player? right) => Equals(left, right);

    public static bool operator !=(Player? left, Player? right) => !Equals(left, right);
}