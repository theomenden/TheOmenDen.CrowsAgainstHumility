using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models;

public sealed record PlayerDto(Guid Id, string Username, IEnumerable<WhiteCard> WhiteCards, int AwesomePoints = 2)
{
    public WhiteCard? PlayedWhiteCard { get; init; }

    public GameRoles GameRole { get; init; } = GameRoles.Player;
}
