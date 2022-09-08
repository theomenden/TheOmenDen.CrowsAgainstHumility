using System.Linq;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models;

public sealed class Player: IEquatable<Player>, IComparable<Player>
{
    #region Private fields
    private Dictionary<Guid, WhiteCard> _whiteCards;

    private bool _isCardCzar = false;

    private Int32 _pointsWon = 2;
    #endregion
    #region Properties
    public IDictionary<Guid, WhiteCard> WhiteCards => _whiteCards;

    public Boolean IsCardCzar => _isCardCzar;

    public Int32 AwesomePoints => _pointsWon; 

    public Guid Id { get; }
    #endregion
    #region Constructors
    public Player()
    {
        Id = Guid.NewGuid();
        _whiteCards = new Dictionary<Guid, WhiteCard>(10);
    }

    public Player(bool isCardCzar)
    {
        Id= Guid.NewGuid();
        _whiteCards = new Dictionary<Guid, WhiteCard>(10);
        _isCardCzar = isCardCzar;
    }
    
    public Player(bool isCardCzar, Dictionary<Guid, WhiteCard> whiteCards)
    {
        Id = Guid.NewGuid();
        _isCardCzar = isCardCzar;
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
    {
        if(_whiteCards.TryGetValue(cardId, out var whiteCard))
        {
            return whiteCard;
        }

        return default;
    }

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
    {

        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }


        return obj is Player player && Equals(player);
    }

    public bool Equals(Player? other) => other is not null
                                        && Id == other.Id;

    public override int GetHashCode() => Id.GetHashCode();

    public int CompareTo(Player? other) => _pointsWon.CompareTo(other?.AwesomePoints ?? 0);

    public static bool operator ==(Player left, Player right) => left is null ? right is null : left.Equals(right);

    public static bool operator !=(Player left, Player right) => !(left == right);
    

    public static bool operator <(Player left, Player right) => left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(Player left, Player right) => left is null || left.CompareTo(right) <= 0;
    

    public static bool operator >(Player left, Player right) => left is not null && left.CompareTo(right) > 0;
    

    public static bool operator >=(Player left, Player right) => left is null ? right is null : left.CompareTo(right) >= 0;
    
}