using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Core.Results;
public sealed class RoundResult : ICollection<KeyValuePair<Player, WhiteCard?>>
{
    #region Private Fields
    private readonly Dictionary<Player, WhiteCard?> _whiteCards = new();
    private bool _isReadOnly;
    #endregion
    #region Constructors
    public RoundResult(IEnumerable<Player> players)
    {
        if (players is null)
        {
            throw new ArgumentNullException(nameof(players));
        }

        foreach (var player in players)
        {
            _whiteCards.Add(player, null);
        }
    }
    #endregion
    #region Indexers
    public WhiteCard? this[Player player]
    {
        get => !ContainsMember(player) ? throw new KeyNotFoundException() : _whiteCards[player];
        set
        {
            if (_isReadOnly)
            {
                throw new InvalidOperationException();
            }

            if (!ContainsMember(player))
            {
                throw new KeyNotFoundException();
            }

            _whiteCards[player] = value;
        }
    }
    #endregion
    #region Public Members & Methods
    public int Count => _whiteCards.Count;
    public bool IsReadOnly => _isReadOnly;
    public bool ContainsMember(Player player) => _whiteCards.ContainsKey(player);
    public void SetReadOnly() => _isReadOnly = true;
    #endregion
    #region ICollection Overrides
    public IEnumerator<KeyValuePair<Player, WhiteCard?>> GetEnumerator() => _whiteCards.GetEnumerator();
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    bool ICollection<KeyValuePair<Player, WhiteCard?>>.Contains(KeyValuePair<Player, WhiteCard?> item) => ((ICollection<KeyValuePair<Player, WhiteCard?>>)_whiteCards).Contains(item);
    bool ICollection<KeyValuePair<Player, WhiteCard?>>.Remove(KeyValuePair<Player, WhiteCard?> item) => throw new NotSupportedException();
    void ICollection<KeyValuePair<Player, WhiteCard?>>.CopyTo(KeyValuePair<Player, WhiteCard?>[] array, int arrayIndex) => ((ICollection<KeyValuePair<Player, WhiteCard?>>)_whiteCards).CopyTo(array, arrayIndex);
    void ICollection<KeyValuePair<Player, WhiteCard?>>.Add(KeyValuePair<Player, WhiteCard?> item) => throw new NotSupportedException();
    void ICollection<KeyValuePair<Player, WhiteCard?>>.Clear() => throw new NotSupportedException();
    #endregion
    #region Internal Data Methods
    internal IDictionary<string, WhiteCard?> GetData()
        => _whiteCards
                .ToDictionary(player => player.Key.Username, player => player.Value);
    #endregion
}
