namespace TheOmenDen.CrowsAgainstHumility.Core.Models.Cards;

public sealed class Deck<T>(IEnumerable<T> cards) : IDisposable where T : BaseImmutableCard
{
    private readonly List<T> _cards = new(cards);
    private readonly List<T> _discards = new();
    private int _currentCardIndex = 0;
    private readonly Random _random = new();
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public void AddCard(T card)
    {
        _cards.Add(card);
    }

    public bool IsEmpty => _currentCardIndex >= _cards.Count;

    public IEnumerable<T> Cards => _cards;

    public async Task<T> DrawCardAsync(CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            if (_currentCardIndex >= _cards.Count)
            {
                if (_discards.Count == 0)
                {
                    throw new InvalidOperationException("Deck is empty");
                }

                await ReshuffleDiscardsIntoDeckAsync(cancellationToken);
                _currentCardIndex = 0;
            }

            return _cards[_currentCardIndex++];
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void Shuffle()
    {
        var n = _cards.Count;
        while (n > 1)
        {
            n--;
            var k = _random.Next(n + 1);
            (_cards[k], _cards[n]) = (_cards[n], _cards[k]);
        }
    }

    public async Task AddToDiscardPileAsync(T card, CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            _cards.Remove(card);
            _discards.Add(card);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void Dispose() => _semaphore?.Dispose();

    private async Task ReshuffleDiscardsIntoDeckAsync(CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            if (_discards.Count == 0)
            {
                throw new InvalidOperationException("Discard pile is empty");
            }

            _cards.AddRange(_discards);
            Shuffle();
            _discards.Clear();
        }
        finally
        {
            _semaphore.Release();
        }
    }
}