namespace TheOmenDen.CrowsAgainstHumility.Core.Models;
public sealed record RoomStateDto(Guid RoomId, IEnumerable<Player> Players)
{
    public IReadOnlyList<Player> CurrentPlayers => Players.Where(player => !player.IsCardCzar).ToList();
    
    public Player? CurrentCardTsar { get; init; } = Players.FirstOrDefault(player => player.IsCardCzar);
    
    public bool ShouldShowCards { get; init; }
    
    public bool ShouldRevealCardsAutomatically { get; init; } = true;
    
    public BlackCard CurrentBlackCard { get; init; }
    
    public IEnumerable<WhiteCard> WhiteCards { get; init; } = Enumerable.Empty<WhiteCard>();
    
    public IEnumerable<BlackCard> BlackCards { get; init; } = Enumerable.Empty<BlackCard>();
    
    public IEnumerable<WhiteCard> PlayedWhiteCards { get; init; } = Enumerable.Empty<WhiteCard>();
    
    public IEnumerable<BlackCard> PlayedBlackCards { get; init; } = Enumerable.Empty<BlackCard>();
    
    public DateTime? WhiteCardTurnTime { get; init; }
    
    public DateTime? CardTsarVotingTurnTime { get; init; }
    
    public String Name { get; init; } = String.Empty;
    
    public String Code { get; init; } = "TEST-1";
}
