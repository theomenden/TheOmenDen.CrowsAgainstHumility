namespace TheOmenDen.CrowsAgainstHumility.Core.Models;

public sealed class Player : IEquatable<Player>, IComparable<Player>, IComparable
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

    public Int32 DisplayPosition { get; set; } = 0;

    public Guid Id { get; set;}

    public bool IsConnected { get; set; }

    public bool IsCardCzar { get; set; } = false;
    #region IEquatable Implementations
    public bool Equals(Player? other) => other is not null 
                                         && (ReferenceEquals(this, other) 
                                             || ConnectionId == other.ConnectionId);

    public override bool Equals(object? obj) => obj is not null
        && ReferenceEquals(this, obj) 
        || obj is Player other 
        && Equals(other);

    public override int GetHashCode() => ConnectionId.GetHashCode();
    #endregion
    #region IComparable Implementations
    public int CompareTo(Player? other)
    {
        if (ReferenceEquals(this, other))
        {
            return 0;
        }

        return other is null ? 1 : AwesomePoints.CompareTo(other.AwesomePoints);
    }

    public int CompareTo(object? obj)
    {
        if (obj is null)
        {
            return 1;
        }

        if (ReferenceEquals(this, obj))
        {
            return 0;
        }

        return obj is Player other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(Player)}");
    }
    #endregion
    #region Overloaded Operators
    public static bool operator ==(Player? left, Player? right) => Equals(left, right);
    public static bool operator !=(Player? left, Player? right) => !Equals(left, right);
    public static bool operator <(Player? left, Player? right) => Comparer<Player>.Default.Compare(left, right) < 0;
    public static bool operator >(Player? left, Player? right) => Comparer<Player>.Default.Compare(left, right) > 0;
    public static bool operator <=(Player? left, Player? right) => Comparer<Player>.Default.Compare(left, right) <= 0;
    public static bool operator >=(Player? left, Player? right) => Comparer<Player>.Default.Compare(left, right) >= 0;
    #endregion
}