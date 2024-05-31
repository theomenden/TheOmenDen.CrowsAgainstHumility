using System.Collections.Concurrent;
using TheOmenDen.CrowsAgainstHumility.Core.Enums;
using TheOmenDen.CrowsAgainstHumility.Core.Identifiers;
using TheOmenDen.CrowsAgainstHumility.Core.Models.Cards;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models;

public sealed class GameState : IDisposable
{
    private readonly SemaphoreSlim _updateLock = new(1, 1);
    private CancellationTokenSource _cancellationTokenSource = new();
    public ConcurrentDictionary<PlayerId, PlayerState> Players { get; private set; } = [];
    public ConcurrentQueue<ImmutableWhiteCard> WhiteCardDeck { get; private set; } = [];
    public ConcurrentQueue<ImmutableBlackCard> BlackCardDeck { get; private set; } = [];
    public ConcurrentDictionary<PlayerId, SortedList<int, ImmutableWhiteCard>> PlayerSubmittedWhiteCards { get; private set; } = [];
    public ImmutableBlackCard CurrentBlackCard { get; private set; }
    public GameStatus Status { get; private set; } = GameStatus.WaitingToStart;
    public PlayerState CurrentJudge { get; private set; }

    public async Task DrawBlackCardAsync(CancellationToken cancellationToken = default)
    {
        await _updateLock.WaitAsync(cancellationToken);
        try
        {
            CurrentBlackCard = BlackCardDeck.TryDequeue(out var card) ? card : throw new InvalidOperationException("No black cards left");
        }
        finally
        {
            _updateLock.Release();
        }
    }

    public async Task StartGameAsync(CancellationToken cancellationToken = default)
    {
        await _updateLock.WaitAsync(cancellationToken);
        try
        {
            Status = GameStatus.InProgress;
            CurrentJudge = Players.Values.First();
            await DealWhiteCardsAsync(cancellationToken);
        }
        finally
        {
            _updateLock.Release();
        }
    }

    public async Task UpdateGameStatusAsync(GameStatus status, CancellationToken cancellationToken = default)
    {
        await _updateLock.WaitAsync(cancellationToken);
        try
        {
            Status = status;
        }
        finally
        {
            _updateLock.Release();
        }
    }

    public async Task UpdateScoresAsync(PlayerId player, int scoreToAdd, CancellationToken cancellationToken = default)
    {
        await _updateLock.WaitAsync(cancellationToken);
        try
        {
            if (Players.TryGetValue(player, out var playerState))
            {
                playerState.AddScore(scoreToAdd);
            }
        }
        finally
        {
            _updateLock.Release();
        }
    }

    public async Task<bool> TryDrawWhiteCardAsync(PlayerId playerId, CancellationToken cancellationToken = default)
    {
        await _updateLock.WaitAsync(cancellationToken);
        try
        {
            if (WhiteCardDeck.TryDequeue(out var card) && Players.TryGetValue(playerId, out var player))
            {
                var updatedHand = new List<ImmutableWhiteCard>(player.Hand) { card };

                var updatedPlayerState = player.AdjustHand(updatedHand);

                Players[playerId] = updatedPlayerState;

                return true;
            }
            return false;
        }
        finally
        {
            _updateLock.Release();
        }
    }

    public async Task SetCurrentJudgeAsync(PlayerId playerId, CancellationToken cancellationToken = default)
    {
        await _updateLock.WaitAsync(cancellationToken);
        try
        {
            CurrentJudge = Players[playerId];
        }
        finally
        {
            _updateLock.Release();
        }
    }

    public void InitializeDecks(Deck<ImmutableWhiteCard> whiteCardDeck, Deck<ImmutableBlackCard> blackCardDeck)
    {
        WhiteCardDeck = new ConcurrentQueue<ImmutableWhiteCard>(whiteCardDeck.Cards);
        BlackCardDeck = new ConcurrentQueue<ImmutableBlackCard>(blackCardDeck.Cards);
    }

    public void ResetGame(CancellationToken cancellationToken)
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public void Dispose()
    {
        _updateLock?.Dispose();
        _cancellationTokenSource?.Dispose();
    }
}