using System.Collections.Concurrent;
using System.Reflection.Metadata.Ecma335;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;
public sealed class CrowGame
{
    #region Constructor
    public CrowGame(IEnumerable<Pack> packs)
    {
        IsShown = false;
        PlayedWhiteCards = new ConcurrentDictionary<int, WhiteCard>();
        Packs = packs;
    }
    #endregion
    #region Properties
    public bool IsShown { get; set; }
    public bool CanClear => PlayedWhiteCards.Any();
    public bool CanPlayWhiteCard => !IsShown;
    public IDictionary<int, WhiteCard> PlayedWhiteCards { get; set; }
    public IEnumerable<Pack> Packs { get; set; } = Enumerable.Empty<Pack>();
    #endregion
    #region Public Methods
    public bool CanShow(IDictionary<WhiteCard, Player> participants)
        => PlayedWhiteCards.Count is not 0
           && !IsShown
           && AllPlayersHavePlayedAWhiteCard(participants);
    public IEnumerable<WhiteCard> GetWhiteCards() => Packs?.SelectMany(p => p.WhiteCards).ToList() ?? Enumerable.Empty<WhiteCard>();
    public IEnumerable<BlackCard> GetBlackCards() => Packs?.SelectMany(p => p.BlackCards).ToList() ?? Enumerable.Empty<BlackCard>();
    #endregion
    #region Private Methods
    private bool AllPlayersHavePlayedAWhiteCard(IDictionary<WhiteCard, Player> participants)
    {
        var currentPlayers = participants.Values.Where(p => !p.IsCardCzar);
        return currentPlayers.All(id => PlayedWhiteCards.ContainsKey(id.PlayedWhiteCard));
    }
    #endregion
}
