using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Core.Rules;
using TheOmenDen.CrowsAgainstHumility.Services.CrowGameBuilder.Interfaces;

namespace TheOmenDen.CrowsAgainstHumility.Services.CrowGameBuilder;

public sealed class CrowGameBuilder : ICanCreateAGame, ICanAddPlayersToAGame, 
    ICanAddCardPacksToAGame, ICanFilterCardsInAPack, ICanCreateARuleSetForAGame
{
    private readonly CrowGame _gameToBuild;

    public CrowGameBuilder()
    {
        _gameToBuild = new();
    }

    public ICanCreateARuleSetForAGame FromTheseRules()
    {
        return this;
    }

    public ICanAddPlayersToAGame WithThesePlayers()
    {
        return this;
    }

    public ICanAddCardPacksToAGame WithTheseCardPacks()
    {
        return this;
    }

    public CrowGame BuildGame()
    {
        return _gameToBuild;
    }

    public ICanAddPlayersToAGame WithPlayers(IEnumerable<string> playerNames)
    {
        return this;
    }

    public ICanCreateAGame AndVerifyPlayersInTheList(IEnumerable<Player> players)
    {
        return this;
    }

    public ICanFilterCardsInAPack OnlyOfficialPacks()
    {
        return this;
    }

    public ICanFilterCardsInAPack ASpecificCardPack(Guid packId)
    {
        return this;
    }

    public ICanFilterCardsInAPack SpecificCardPacks(IEnumerable<Guid> packIds)
    {
        return this;
    }

    public ICanCreateAGame WithNoFilters()
    {
        return this;
    }

    public ICanCreateAGame WithDefaultFilters()
    {
        return this;
    }

    public ICanAddCardPacksToAGame AddingAdditionalWordFilters(IEnumerable<string> wordsToExclude)
    {
        return this;
    }

    public ICanAddCardPacksToAGame AddingSpecificCardFilters(string nameToExclude)
    {
        return this;
    }

    public ICanCreateAGame WithABaseRuleSet(GameTypes gameType)
    {
        return this;
    }

    public ICanCreateARuleSetForAGame WithAnAlternateWinCondition()
    {
        return this;
    }

    public ICanCreateARuleSetForAGame WithAdditionalHouseRules()
    {
        return this;
    }
}

