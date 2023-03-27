using System.Collections.Concurrent;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;

namespace TheOmenDen.CrowsAgainstHumility.Services.Lobbies;
public sealed class CrowGameLobby
{
    private readonly IDictionary<Guid, CrowGame> _games;

    public CrowGameLobby()
    {
        _games = new ConcurrentDictionary<Guid, CrowGame>();
    }

    public CrowGame Create(IList<WhiteCard> whiteCards, IList<BlackCard> blackCards)
    {
        var newGameId = Guid.NewGuid();
        var newGame = new CrowGame(whiteCards, blackCards);
        
        _games.Add(newGameId, newGame);

        return newGame;
    }

    public ICollection<CrowGame> GetAll()
    {
        return _games.Values;
    }

    public void Remove(CrowGame game)
    {
        _games.Remove(game.Id);
    }
}
