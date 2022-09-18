namespace TheOmenDen.CrowsAgainstHumility.Services.CrowGameBuilder.Interfaces;
public interface ICanFilterCardsInAPack
{
    /// <summary>
    /// Allows for all cards in a pack to be used
    /// </summary>
    /// <returns>><see cref="ICanAddCardPacksToAGame"/> for further chaining</returns>
    ICanCreateAGame WithNoFilters();

    /// <summary>
    /// Enables our default filters for words
    /// </summary>
    /// <returns><see cref="ICanAddCardPacksToAGame"/> for further chaining</returns>
    ICanCreateAGame WithDefaultFilters();

    /// <summary>
    /// Allows for additional word filters to be defined
    /// </summary>
    /// <param name="wordsToExclude">Additional words we want to exclude</param>
    /// <returns><see cref="ICanAddCardPacksToAGame"/> for further chaining</returns>
    ICanAddCardPacksToAGame AddingAdditionalWordFilters(IEnumerable<String> wordsToExclude);

    /// <summary>
    /// Allows for filtering out a specific card
    /// </summary>
    /// <param name="nameToExclude">The name of the card to exclude</param>
    /// <returns><see cref="ICanAddCardPacksToAGame"/> for further chaining</returns>
    ICanAddCardPacksToAGame AddingSpecificCardFilters(String nameToExclude);
}

