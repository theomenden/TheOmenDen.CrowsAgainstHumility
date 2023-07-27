using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Core.Providers;

namespace TheOmenDen.CrowsAgainstHumility.Azure.Hubs;
public class CrowGameHub : Hub
{
    private readonly ConcurrentDictionary<string, CrowGameLobby> _games = new();

    private readonly ICardProvider _cardProvider;

    public CrowGameHub(ICardProvider cardProvider)
    {
        _cardProvider = cardProvider;
    }

    public async Task GetGameState(string groupName, CancellationToken cancellationToken = default)
    {
        if (_games.TryGetValue(groupName, out var game))
        {
            var user = Context.UserIdentifier;
            var hand = game.Hands[user];
            var playedCards = game.PlayedCards;
            var czar = game.CardTsar;
            var currentBlackCard = game.CurrentBlackCard;

            await Clients.Caller.SendAsync("ReceiveGameState", hand, playedCards, czar, currentBlackCard, cancellationToken);

        }

        public async Task JoinGame(string groupName)
        {
            var user = Context.UserIdentifier;
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName, cancellationToken);

            if (!_games.TryGetValue(groupName, out var game))
            {
                game = new CrowGame();
                _games[groupName] = game;
            }

            game.Hands[user] = DrawWhiteCards(7);
            await Clients.Group(groupName).SendAsync("ReceiveMessage", game.Hands[user]);
        }

        public async Task LeaveGame(string groupName, CancellationToken cancellationToken = default)
        {
            var await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            if (_games.TryGetValue(groupName, out var game))
            {
                game.Hands.TryRemove(user, out _);
            }

            await Clients.Group(groupName).SendAsync("ReceiveMessage", user, cancellationToken);
        }
    }

    public async Task ChooseWinnerAsync(string groupName, string winner, CancellationToken cancellationToken = default)
    {
        var user = Context.UserIdentifier;

        if (_games.TryGetValue(groupName, out var game)
            && game.CardTsar == user)
        {
            game.CardTsar = winner;
            game.CurrentBlackCard = _cardProvider.DrawBlackCard();
            game.PlayedCards.Clear();

            foreach (var player in game.Hands.Keys)
            {
                var cardsToDraw = 7 - game.Hands[player].Count; //Ensure we always have 7 cards.

                if (cardsToDraw <= 0) continue;
                var newCards = _cardProvider.DrawWhiteCards(cardsToDraw);
                game.Hands[player].AddRange(newCards);
            }

            await Clients.Group(groupName).SendAsync("ReceiveMessage", $"[SERVER]: {winner} won the round. The new Czar is {game.CardTsar}.", cancellationToken);
        }
    }
}
