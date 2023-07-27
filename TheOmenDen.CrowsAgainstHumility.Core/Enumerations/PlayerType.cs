using TheOmenDen.Shared.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.DTO.Enums;

public sealed record GameRole : EnumerationBase<GameRole>
{
    private GameRole(string name, int id) : base(name, id) { }

    public static readonly GameRole Player = new(nameof(Player), 1);
    public static readonly GameRole CardTsar = new(nameof(CardTsar), 2);
    public static readonly GameRole Observer = new(nameof(Observer), 3);
}