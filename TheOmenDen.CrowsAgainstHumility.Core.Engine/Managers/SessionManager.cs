using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Cards;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Game;

namespace TheOmenDen.CrowsAgainstHumility.Core.Engine.Managers;
internal static class SessionManager
{
    #region Public Methods
    public static bool HasPlayedACard(CrowGameSession session, int playerPublicId) => session.PlayedCards.ContainsKey(playerPublicId);
    #endregion
    #region Internal Methods
    internal static void PlayWhiteCard(CrowGameSession session, int playerPublicId, WhiteCard whiteCard) => session.PlayedCards[playerPublicId] = whiteCard;
    internal static void UnplayWhiteCard(CrowGameSession session, int playerPublicId) => session.PlayedCards.Remove(playerPublicId);
    internal static void ShowCards(CrowGameSession session) => session.IsShown = true;
    internal static void RemovePlayer(CrowGameSession session, int playerPublicId)
    {
        session.PlayedCards.Remove(playerPublicId);
        if (!session.PlayedCards.Any())
        {
            session.IsShown = false;
        }
    }
    internal static void ClearGameBoard(CrowGameSession session)
    {
        session.PlayedCards = new Dictionary<int, WhiteCard>();
        session.IsShown = false;
    }
    #endregion
}
