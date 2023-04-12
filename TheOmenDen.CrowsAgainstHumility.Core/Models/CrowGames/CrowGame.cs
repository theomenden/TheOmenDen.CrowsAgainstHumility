using System.Collections.Concurrent;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;
public sealed class CrowGame
{
    #region Constructor
    public CrowGame(Deck deck)
    {
        IsShown = false;
        PlayedWhiteCards = new ConcurrentDictionary<Guid, WhiteCard>();
        Deck = deck;
    }
    #endregion
    #region Properties
    public bool IsShown { get; set; }
    public bool CanClear => PlayedWhiteCards.Any();
    public bool CanPlayWhiteCard => !IsShown;
    public IDictionary<Guid, WhiteCard> PlayedWhiteCards { get; set; }
    public Deck Deck { get; set; }
    #endregion
    #region Public Methods
    public bool CanShow(IDictionary<WhiteCard, Member> participants)
        => PlayedWhiteCards.Count is not 0
           && !IsShown
           && AllPlayersHavePlayedAWhiteCard(participants);
    public IEnumerable<WhiteCard> GetWhiteCards() => Deck?.WhiteCards.ToArray() ?? Enumerable.Empty<WhiteCard>();
    public IEnumerable<BlackCard> GetBlackCards() => Deck?.BlackCards.ToArray() ?? Enumerable.Empty<BlackCard>();
    #endregion
    #region Private Methods
    private bool AllPlayersHavePlayedAWhiteCard(IDictionary<WhiteCard, Member> participants)
    {
        var currentPlayers = participants.Values.Where(p => !p.IsDormant);
        return currentPlayers.All(member => PlayedWhiteCards.ContainsKey(member.SessionId));
    }
    #endregion
}
