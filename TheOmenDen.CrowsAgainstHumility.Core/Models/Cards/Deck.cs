namespace TheOmenDen.CrowsAgainstHumility.Core.Models.Cards;

public class Deck<T> : IDisposable where T : BaseCard
{
    private readonly List<T> _cards = new();
    private readonly List<T> _discards = new();
    private readonly Random _random = new();
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public void AddCard(T card)
    {
        _cards.Add(card);
    }

    public T DrawCard()
    {
        _semaphore.Wait();
        try
        {
            if (_cards.Count == 0)
            {
                throw new InvalidOperationException("Deck is empty");
            }

            var index = _random.Next(_cards.Count);
            var card = _cards[index];
            _cards.RemoveAt(index);
            return card;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<T> DrawCardAsync(CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            if (_cards.Count == 0)
            {
                if (_discards.Count == 0)
                {
                    throw new InvalidOperationException("Deck is empty");
                }

                await ReshuffleDiscardsIntoDeckAsync(cancellationToken);
            }

            var index = _random.Next(_cards.Count);
            var card = _cards[index];
            _cards.RemoveAt(index);
            return card;
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

    public async Task AddToDiscardPileAsync(T card)
    {
        await _semaphore.WaitAsync();
        try
        {
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