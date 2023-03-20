using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models;

public sealed record Player : IComparable<Player>, IComparable
{
    private const int MaxHandSize = 10;
    
    public String ConnectionId { get; init; }

    public GameRoles GameRole { get; init; } = GameRoles.Player;

    public int AwesomePoints { get; init; } = 2;

    public string Name { get; init; } = String.Empty;

    public string Username { get; init; } = String.Empty;

    public WhiteCard? PlayedWhiteCard { get; init; }

    public IEnumerable<WhiteCard> Hand { get; init; } = Enumerable.Empty<WhiteCard>();

    public Int32 DisplayPosition { get; init; } = 0;

    public Guid Id { get; init;}

    public bool IsConnected { get; init; }

    public bool IsCardCzar => GameRole == GameRoles.CardTsar;
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
    public static bool operator <(Player? left, Player? right) => Comparer<Player>.Default.Compare(left, right) < 0;
    public static bool operator >(Player? left, Player? right) => Comparer<Player>.Default.Compare(left, right) > 0;
    public static bool operator <=(Player? left, Player? right) => Comparer<Player>.Default.Compare(left, right) <= 0;
    public static bool operator >=(Player? left, Player? right) => Comparer<Player>.Default.Compare(left, right) >= 0;
    #endregion
}