using System.Collections.Concurrent;
using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Cards;
using TheOmenDen.CrowsAgainstHumility.Core.DTO.Models.Cards;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Models;

namespace TheOmenDen.CrowsAgainstHumility.Core.Engine.Game;
public sealed class CrowGameSession
{
    public CrowGameSession(Deck cardSet)
    {
        IsShown = false;
        PlayedCards = new ConcurrentDictionary<int, WhiteCard>();
        CardSet = cardSet;
    }

    public bool IsShown { get; set; }
    public bool CanClear => PlayedCards.Any();
    public bool CanPlay => !IsShown;
    public Deck CardSet { get; set; }
    public IDictionary<int, WhiteCard> PlayedCards { get; set; }

    public bool CanShow(IDictionary<string, Player> participants) => false;

    private bool AllAwakeParticipantsPlayed(IDictionary<string, Player> participants)
    {
        var awakeParticipants = participants.Values
            .Where(p => p.Type == GameRoles.Player 
                        && p.Mode == PlayerMode.Awake)
            .Select(p => p.PublicId);
        return awakeParticipants.All(id => PlayedCards.ContainsKey(id));
    }
}
