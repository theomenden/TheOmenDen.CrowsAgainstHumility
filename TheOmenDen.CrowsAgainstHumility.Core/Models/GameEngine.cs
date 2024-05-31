using TheOmenDen.CrowsAgainstHumility.Core.Enums;
using TheOmenDen.CrowsAgainstHumility.Core.Models.Cards;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models;

public sealed class GameEngine
{
    public GameState GameState { get; private set; }

    public GameEngine(GameState gameState)
    {
        GameState = gameState;
    }

    public async Task SetupGame(GameSessionDto session, CancellationToken cancellationToken = default)
    {
        GameState.Players = session.Players.Select(player => new Player(player.Id, player.Name)).ToList();
        GameState.WhiteCardDeck = new Deck<ImmutableWhiteCard>(session.WhiteCards);
        GameState.BlackCardDeck = new Deck<ImmutableBlackCard>(session.BlackCards);
        await GameState.StartGameAsync(cancellationToken);
    }

    public async Task ProcessPlayerMoveAsync(Guid sessionId, Guid playerId, ImmutableWhiteCard card, CancellationToken cancellationToken = default)
    {
        await GameState.TrySubmitCardAsync(playerId, card, cancellationToken);
    }

    public async Task UpdateGameStateAsync(GameSessionDto session, CancellationToken cancellationToken = default)
    {
        if (GameState.Players.All(player => GameState.PlayerSubmittedWhiteCards.ContainsKey(player.Id)))
        {
            await GameState.MoveToNextRoundAsync(cancellationToken);
        }
    }

    public bool IsGameOver => GameState.Status == GameStatus.Completed;

    public async Task FinalizeGameAsync(GameSessionDto session, CancellationToken cancellationToken = default)
    {
        GameState.SetStatus(GameStatus.Completed);
        await Task.CompletedTask;
    }
}