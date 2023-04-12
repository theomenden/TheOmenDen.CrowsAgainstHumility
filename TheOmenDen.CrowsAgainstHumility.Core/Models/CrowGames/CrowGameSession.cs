using System.Collections.Concurrent;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;
public class CrowGameSession
{
    public CrowGameSession(Deck deck)
    {
        IsShown = false;
        Deck = deck;
        PlayedCards = new ConcurrentDictionary<Guid, WhiteCard>();
    }
    #region Public Properties
    public bool IsShown { get; set; }
    public bool CanClearBoard => PlayedCards.Any();
    public bool CanPlayCards => !IsShown;
    public IDictionary<Guid, WhiteCard> PlayedCards { get; set; }
    public Deck Deck { get; set; }
    #endregion
    #region Public Methods
    public bool CanShowCards(IDictionary<string, Player> players)
    => PlayedCards.Count is not 0
    && !IsShown
    && AllAwakePlayersHavePlayedACard(players);
    #endregion
    #region Private Methods
    private bool AllAwakePlayersHavePlayedACard(IDictionary<string, Player> players)
    {
        var awakePlayers = players.Values.Where(p => p.GameRole == GameRoles.Player
                                                     && p.Mode == PlayerMode.Awake)
            .Select(p => p.Id);
        return awakePlayers.All(id => PlayedCards.ContainsKey(id));
    }
    #endregion
}
