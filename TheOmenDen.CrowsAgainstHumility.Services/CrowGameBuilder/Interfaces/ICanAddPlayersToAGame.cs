using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Services.CrowGameBuilder.Interfaces;
public interface ICanAddPlayersToAGame
{

    /// <summary>
    /// Add a list of players by their names/handles to a <see cref="CrowGame"/>
    /// </summary>
    /// <param name="playerNames">The list of player handles</param>
    /// <returns><see cref="ICanAddPlayersToAGame"/> for further functionality</returns>
    ICanAddPlayersToAGame WithPlayers(IEnumerable<String> playerNames);
    
    /// <summary>
    /// Queries relevant APIs to make sure that only players in initial list are present in the game
    /// </summary>
    /// <returns><see cref="ICanCreateAGame"/> interface to allow for further chaining</returns>
    ICanCreateAGame AndVerifyPlayersInTheList(IEnumerable<Player> players);
}
