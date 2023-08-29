using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Cards;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Core.Providers;

namespace TheOmenDen.CrowsAgainstHumility.Core.Engine.Game;

public interface IGameService
{
    Task<CrowGame> CreateGame(Guid czarId, BlackCard blackCard, CancellationToken cancellationToken = default);
    Task JoinGameAsync(CrowGame game, Guid playerId, CancellationToken cancellationToken = default);
    Task LeaveGameAsync(CrowGame game, Guid playerId, CancellationToken cancellationToken = default);
    Task PlayCardAsync(CrowGame game, Guid playerId, WhiteCard card, CancellationToken cancellationToken = default);
    Task ChooseWinnerAsync(CrowGame game, Guid playerId, Guid winnerId, CancellationToken cancellationToken = default);
}
internal class GameService : IGameService
{
    private readonly ICardProvider _cardProvider;
    private readonly ILogger<GameService> _logger;

    public async Task<CrowGame> CreateGame(Guid czarId, BlackCard blackCard, CancellationToken cancellationToken = default)
    {
        var game = new CrowGame(czarId, blackCard, new ConcurrentDictionary<Guid, CrowGamePlayer>(),
            DateTimeOffset.UtcNow, TimeSpan.FromMinutes(12));

        _logger.LogInformation("Created new Game {Game} with initial Czar {Czar}", game, czarId);

        return game;
    }

    public async Task JoinGameAsync(CrowGame game, Guid playerId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task LeaveGameAsync(CrowGame game, Guid playerId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task PlayCardAsync(CrowGame game, Guid playerId, WhiteCard card, CancellationToken cancellationToken = default)
    {
        if (game.ConnectedPlayers.TryGetValue(playerId, out var player))
        {
            player = player with { PlayedCard = card };
            _logger.LogInformation("Player {PlayerId} played card {CardId}", playerId, card.Id);

            game.ConnectedPlayers[playerId] = player;


            var randoCardrissian = game.ConnectedPlayers.First(p => p.Key == Guid.Empty).Value;
            randoCardrissian = randoCardrissian with { PlayedCard = _cardProvider.DrawWhiteCards(1).First() };
            _logger.LogInformation("Rando Cardrissian played a card in game {Game}", game);

            return;
        }

        _logger.LogError("Player with Id {Id} was not found in game {Game}", playerId, game);
    }

    public async Task ChooseWinnerAsync(CrowGame game, Guid playerId, Guid winnerId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public bool HasTurnTimedOut(CrowGame game)
    => game.TurnStartedAt.Add(game.TurnLength) < DateTimeOffset.UtcNow;
}
