using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using TheOmenDen.CrowsAgainstHumility.Bootstrapping;
using TheOmenDen.CrowsAgainstHumility.Core.Requests;

namespace TheOmenDen.CrowsAgainstHumility.Hubs;


public class CrowGameHub : Hub
{
    public const string HubUrl = "/games";

    private static readonly List<GamePlay> GamePlays = new (100);

    public async Task RegisterPlayerAsync(RegisterPlayerRequest registerPlayerRequest)
    {
        var playerExists = GamePlays.Exists(game => game.HubConnectionId == Context.ConnectionId);

        if (playerExists)
        {
            return;
        }

        var gamePlay = new GamePlay
        {
            HubConnectionId = Context.ConnectionId,
            Game = registerPlayerRequest.Game,
            Player = registerPlayerRequest.Player,
            PlayedCard = new()
        };

        GamePlays.Add(gamePlay);

        await Groups.AddToGroupAsync(Context.ConnectionId, registerPlayerRequest.Game.Id.ToString());

        await UpdateGame(gamePlay.Game);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var gamePlay = GamePlays.FirstOrDefault(game => game.HubConnectionId == Context.ConnectionId);

        if (gamePlay is not null)
        {
            GamePlays.Remove(gamePlay);

            await UpdateGame(gamePlay.Game);
        }

        await base.OnDisconnectedAsync(exception);
    }

    private async Task UpdateGame(CrowGame game)
    {
        var gamePlaysForGame = GamePlays.Where(gp => gp.Game.Id == game.Id).ToArray();

        await Clients.Group(game.Name).SendAsync("UpdateGame", gamePlaysForGame);
    }
}
