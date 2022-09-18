using TheOmenDen.CrowsAgainstHumility.Core.Rules;

namespace TheOmenDen.CrowsAgainstHumility.Services.CrowGameBuilder.Interfaces;
public interface ICanCreateARuleSetForAGame
{
    /// <summary>
    /// With rules according to the supplied <paramref name="gameType"/>
    /// </summary>
    /// <param name="gameType">The base rule determinations</param>
    /// <returns><see cref="ICanCreateAGame"/> for further chaining</returns>
    ICanCreateAGame WithABaseRuleSet(GameTypes gameType);

    /// <summary>
    /// Allows an alternate Win Condition to be specified for a game
    /// </summary>
    /// <returns><see cref="ICanCreateAGame"/> for further chaining</returns>
    ICanCreateARuleSetForAGame WithAnAlternateWinCondition();

    /// <summary>
    /// Allows for additional house rules to be added to a game
    /// </summary>
    /// <returns><see cref="ICanCreateAGame"/> for further chaining</returns>
    ICanCreateARuleSetForAGame WithAdditionalHouseRules();
}

