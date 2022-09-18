using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Services.CrowGameBuilder.Interfaces;
public interface ICanAddCardPacksToAGame
{
    /// <summary>
    /// Adds only the official <see cref="Pack"/>s to the game
    /// </summary>
    /// <returns></returns>
    ICanFilterCardsInAPack OnlyOfficialPacks();

    /// <summary>
    /// Adds a single <see cref="Pack"/> to the game based off of the <paramref name="packId"/> supplied
    /// </summary>
    /// <param name="packId">The pack Identifier</param>
    /// <returns><see cref="ICanAddCardPacksToAGame"/> for further functionality</returns>
    ICanFilterCardsInAPack ASpecificCardPack(Guid packId);

    /// <summary>
    /// Adds <see cref="Pack"/>s to the game based off of their <paramref name="packIds"/> supplied
    /// </summary>
    /// <param name="packIds">The pack Identifiers</param>
    /// <returns><see cref="ICanAddCardPacksToAGame"/> for further functionality</returns>
    ICanFilterCardsInAPack SpecificCardPacks(IEnumerable<Guid> packIds);
}
