using TheOmenDen.CrowsAgainstHumility.Core.DTO.Models.Cards;
using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;

namespace TheOmenDen.CrowsAgainstHumility.Services.Managers;
internal static class SessionManager
{
    #region Internal Methods 
    internal static void RemovePlayedCard(CrowGameSession session, int playerPublicId) => session.PlayedCards.Remove(playerPublicId);
    internal static void PlayWhiteCard(CrowGameSession session, int playerPublicId, WhiteCardDto card) => session.PlayedCards[playerPublicId] = card;
    internal static void ShowWhiteCards(CrowGameSession session) => session.IsShown = true;
    internal static void ClearPlayedCards(CrowGameSession session)
    {
        session.PlayedCards = new Dictionary<Guid, WhiteCardDto>();
        session.IsShown = false;
    }
    internal static void RemovePLayer(CrowGameSession session, int playerPublicId)
    {
        session.PlayedCards.Remove(playerPublicId);

        if (!session.PlayedCards.Any())
        {
            session.IsShown = false;
        }
    }
    #endregion
    #region Public Methods
    public static bool HasPlayedACard(CrowGameSession session, int playerPublicId) => session.PlayedCards.ContainsKey(playerPublicId);
    #endregion
}
