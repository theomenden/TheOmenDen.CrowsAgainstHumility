using TheOmenDen.Shared.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
public sealed record GameRules: EnumerationBase<GameRules>
{
    private GameRules(string name, int id) : base(name, id) { }

    public static readonly GameRules Standard = new(nameof(Standard), 1);
    public static readonly GameRules RandoCardrissian = new("Rando Cardrissian", 2);
    public static readonly GameRules Haiku = new(nameof(Haiku), 3);
}
