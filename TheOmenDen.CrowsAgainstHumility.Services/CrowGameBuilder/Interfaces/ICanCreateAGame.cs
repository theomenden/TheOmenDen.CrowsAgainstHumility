using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Services.CrowGameBuilder.Interfaces;

public interface ICanCreateAGame
{
    ICanCreateARuleSetForAGame FromTheseRules();

    /// <summary>
    /// Creates the initial <see cref="CrowGame"/> with players
    /// </summary>
    /// <returns><see cref="ICanAddPlayersToAGame"/> for further chaining</returns>
    ICanAddPlayersToAGame WithThesePlayers();
 
    /// <summary>
    /// Allows consumer to add packs to a game
    /// </summary>
    /// <returns><see cref="ICanAddCardPacksToAGame"/> for further functionality</returns>
    ICanAddCardPacksToAGame WithTheseCardPacks();

    /// <summary>
    /// Returns the completed <see cref="CrowGame"/> object
    /// </summary>
    CrowGame BuildGame();
}
