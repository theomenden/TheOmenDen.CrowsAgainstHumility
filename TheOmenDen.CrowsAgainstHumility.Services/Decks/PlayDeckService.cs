using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Repositories;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;
using TheOmenDen.CrowsAgainstHumility.Core.Specifications;
using TheOmenDen.Shared.Specifications;

namespace TheOmenDen.CrowsAgainstHumility.Services.Decks;
internal class PlayDeckService
{
    private const string CardLoadingMessageTemplate = @"Loaded {WhiteCardCount} white, and {BlackCardcount} black cards from {Pack} for Lobby: {Lobby}";

    private readonly IPackRepository _packs;
    private readonly ILogger<PlayDeckService> _logger;

    public PlayDeckService(IPackRepository packs, ILogger<PlayDeckService> logger)
    {
        _packs = packs;
        _logger = logger;
    }

    public async Task<Deck> GetPlayDeckAsync(string lobbyCode, Specification<Pack> specification, CancellationToken cancellationToken = default)
    {
        var packs = (await _packs.GetAllPacksBySpecificationAsync(specification, cancellationToken)).ToList();
        
        _logger.LogInformation("Loaded {PackCount} packs for Lobby: {Lobby}", packs.Count, lobbyCode);

        var whiteCards = packs.SelectMany(p => p.WhiteCards).ToList();
        var blackCards = packs.SelectMany(p => p.BlackCards).ToList();

        return new Deck(lobbyCode, whiteCards, blackCards);
    }

    public async Task<Deck> GetPlayDeckFromStreamAsync(string lobbyCode, Specification<Pack> specification, CancellationToken cancellationToken = default)
    {

        var whiteCards = new List<WhiteCard>(2000);
        var blackCards = new List<BlackCard>(2000);

        await foreach (var pack in _packs.GetAllPacksThatMatch(specification, cancellationToken))
        {
            whiteCards.AddRange(pack.WhiteCards);
            blackCards.AddRange(pack.BlackCards);

            _logger.LogInformation(CardLoadingMessageTemplate, pack.WhiteCardsInPack, pack.BlackCardsInPack, pack.Name, lobbyCode);
        }

        return new Deck(lobbyCode, whiteCards, blackCards);
    }

    public async Task<Deck> GetAllOfficialPacksForDeckAsync(string lobbyCode, OfficialPacksSpecification specification,
        CancellationToken cancellationToken = default)
    {
        var packs = (await _packs.GetAllPacksBySpecificationAsync(specification, cancellationToken)).ToList();

        _logger.LogInformation("Loaded {PackCount} packs for Lobby: {Lobby}", packs.Count, lobbyCode);

        var whiteCards = packs.SelectMany(p => p.WhiteCards).ToList();
        var blackCards = packs.SelectMany(p => p.BlackCards).ToList();

        return new Deck(lobbyCode, whiteCards, blackCards);
    }
}
