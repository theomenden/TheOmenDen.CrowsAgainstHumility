using TheOmenDen.Shared.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
public sealed record GameRoles: EnumerationBase<GameRoles>
{
    private GameRoles(string name, int id): base(name, id){}

    public static readonly GameRoles Facilitator = new(nameof(Facilitator), 1);
    public static readonly GameRoles Player = new(nameof(Player), 2);
    public static readonly GameRoles CardTsar = new(nameof(CardTsar), 3);
    public static readonly GameRoles Observer = new(nameof(Observer), 4);
}
