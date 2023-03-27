using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;

namespace TheOmenDen.CrowsAgainstHumility.Services.Managers;
internal static class SessionManager
{
    #region Internal Static Methods
    internal static void SetWhiteCard(CrowGame session, int playerPublicId, WhiteCard card)
    => session.PlayedWhiteCards[playerPublicId] = card;

    internal static void RemoveWhiteCard(CrowGame session, int playerPublicId) => session.PlayedWhiteCards.Remove(playerPublicId);

    internal static void Show(CrowGame session) => session.IsShown = true;

    internal static void Clear(CrowGame session)
    {
        session.PlayedWhiteCards.Clear();
        session.IsShown = false;
    }

    internal static void RemovePlayer(CrowGame session, int playerPublicId)
    {
        session.PlayedWhiteCards.Remove(playerPublicId);
        if (!session.PlayedWhiteCards.Any())
        {
            session.IsShown = false;
        }
    }
    #endregion
    #region Public static Methods
    public static bool HasPlayedACard(CrowGame session, int playerPublicId)
        => session.PlayedWhiteCards.ContainsKey(playerPublicId);
    #endregion
}
