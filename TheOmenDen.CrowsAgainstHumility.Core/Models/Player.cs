using System.Linq;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models;

public sealed class Player : IEquatable<Player>, IComparable<Player>
{
    #region Private fields
    private Dictionary<Guid, WhiteCard> _whiteCards;

    private Int32 _pointsWon = 2;
    #endregion
    #region Properties
    public IDictionary<Guid, WhiteCard> WhiteCards => _whiteCards;

    public Boolean IsCardCzar { get; set; }

    public Int32 AwesomePoints { get; set; } = 2;

    public Guid Id { get; }

    public String Username { get; set; } = String.Empty;
    #endregion
    #region Constructors
    public Player()
    : this(new Dictionary<Guid, WhiteCard>(10))
    {}

    public Player(Dictionary<Guid, WhiteCard> whiteCards)
    {
        Id = Guid.NewGuid();
        _whiteCards = whiteCards;
    }
    #endregion

    /// <summary>
    /// Adds a single <paramref name="whiteCard"/> to this player's hand.
    /// </summary>
    /// <param name="whiteCard">The new card</param>
    /// <returns>True if value could be added</returns>
    public Boolean AddWhiteCardToHand(WhiteCard whiteCard)
    {
        _whiteCards ??= new(10);

        return _whiteCards.TryAdd(whiteCard.Id, whiteCard);
    }

    /// <summary>
    /// Adds <paramref name="whiteCards"/> to this player's hand
    /// </summary>
    /// <param name="whiteCards">The cards we want to add</param>
    public void AddWhiteCardsToHand(WhiteCard[] whiteCards)
    {
        _whiteCards ??= new(10);

        Array.ForEach(whiteCards, card =>
        {
            _whiteCards.TryAdd(card.Id, card);
        });
    }

    public WhiteCard? PlayWhiteCardFromHand(Guid cardId)
    => _whiteCards.TryGetValue(cardId, out var whiteCard)
            ? whiteCard
            : default;

    /// <summary>
    /// Allows the player to wager 1 awesome point
    /// </summary>
    public void WagerPoint() => _pointsWon--;

    /// <summary>
    /// Allows the player to add 1 awesome point to their total
    /// </summary>
    public void WinPoint() => _pointsWon++;

    /// <summary>
    /// Checks to see if a player has enough points (more than 0) to wager.
    /// </summary>
    public bool HasPointsToWager => _pointsWon > 0;

    public override bool Equals(object? obj)
    => obj is not null
       && (ReferenceEquals(this, obj) 
           || obj is Player player 
           && Equals(player));
    

    public bool Equals(Player? other) => other is not null
                                        && (Id == other.Id
                                        || other.Username.Equals(Username, StringComparison.OrdinalIgnoreCase)
                                        );

    public override int GetHashCode() => Id.GetHashCode();

    public int CompareTo(Player? other) => _pointsWon.CompareTo(other?.AwesomePoints ?? 0);

    public static bool operator ==(Player? left, Player? right) => left?.Equals(right) ?? right is null;

    public static bool operator !=(Player? left, Player? right) => !(left == right);


    public static bool operator <(Player? left, Player? right) => left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(Player? left, Player? right) => left is null || left.CompareTo(right) <= 0;


    public static bool operator >(Player? left, Player? right) => left is not null && left.CompareTo(right) > 0;


    public static bool operator >=(Player? left, Player? right) => left is null ? right is null : left.CompareTo(right) >= 0;

}