using System.Collections;
using TheOmenDen.CrowsAgainstHumility.Core.DTO.Models.Cards;
using TheOmenDen.CrowsAgainstHumility.Core.DTO.Models.Players;

namespace TheOmenDen.CrowsAgainstHumility.Core.DTO.Results.Domain;
public sealed class RoundResult : ICollection<KeyValuePair<Member, WhiteCardDto?>>
{
    #region Private Fields
    private readonly Dictionary<Member, WhiteCardDto?> _playedCards = new();
    private bool _isReadOnly;
    #endregion
    #region Constructors
    public RoundResult(IEnumerable<Member> members)
    {
        ArgumentNullException.ThrowIfNull(members);

        foreach (var member in members)
        {
            _playedCards.Add(member, null);
        }
    }

    internal RoundResult(Lobby lobby, IDictionary<string, WhiteCardDto?> playResults)
    {
        foreach (var (memberName, value) in playResults)
        {
            var member = lobby.FindMemberOrObserver(memberName) as Member
                         ?? new Member(lobby, memberName);

            _playedCards.Add(member, value);
        }
    }
    #endregion
    #region Indexers
    public WhiteCardDto? this[Member member]
    {
        get =>
            !ContainsMember(member)
                ? throw new KeyNotFoundException(Resources.Resources.PlayerNotInRoundResult)
                : _playedCards[member];
        set
        {
            if (_isReadOnly)
            {
                throw new InvalidOperationException(Resources.Resources.RoundResultIsReadOnly);
            }

            if (!ContainsMember(member))
            {
                throw new KeyNotFoundException(Resources.Resources.PlayerNotInRoundResult);
            }

            _playedCards[member] = value;
        }
    }
    #endregion
    #region Public Methods
    public bool ContainsMember(Member member) => _playedCards.ContainsKey(member);
    public void SetToReadOnly() => _isReadOnly = true;
    #endregion
    #region Properties
    public int Count => _playedCards.Count;
    public bool IsReadOnly => _isReadOnly;
    #endregion
    #region Overrides
    public IEnumerator<KeyValuePair<Member, WhiteCardDto?>> GetEnumerator() => _playedCards.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    void ICollection<KeyValuePair<Member, WhiteCardDto?>>.Add(KeyValuePair<Member, WhiteCardDto?> item)
    {
        throw new NotSupportedException();
    }
    void ICollection<KeyValuePair<Member, WhiteCardDto?>>.Clear()
    {
        throw new NotSupportedException();
    }
    bool ICollection<KeyValuePair<Member, WhiteCardDto?>>.Contains(KeyValuePair<Member, WhiteCardDto?> item)
        => ((ICollection<KeyValuePair<Member, WhiteCardDto?>>)_playedCards).Contains(item);
    void ICollection<KeyValuePair<Member, WhiteCardDto?>>.CopyTo(KeyValuePair<Member, WhiteCardDto?>[] array, int arrayIndex)
    => ((ICollection<KeyValuePair<Member, WhiteCardDto?>>)_playedCards).CopyTo(array, arrayIndex);
    bool ICollection<KeyValuePair<Member, WhiteCardDto?>>.Remove(KeyValuePair<Member, WhiteCardDto?> item)
    {
        throw new NotSupportedException();
    }
    #endregion
    #region Internal Methods
    internal IDictionary<string, WhiteCardDto?> GetData()
    {
        var result = new Dictionary<string, WhiteCardDto?>();

        foreach (var (member, playedCard) in _playedCards)
        {
            result.Add(member.Name, playedCard);
        }

        return result;
    }
    #endregion
}
