using TheOmenDen.Shared.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.Rules;

public sealed record GameTypes : EnumerationBase<GameTypes>
{
    private GameTypes(string name, int id) : base(name, id)
    {
    }

    public static readonly GameTypes Normal = new(nameof(Normal), 0);
    public static readonly GameTypes HappyEnding = new("Happy Ending",1);
    public static readonly GameTypes RebootingTheUniverse = new("Rebooting The Universe", 2);
    public static readonly GameTypes PackingHeat = new("Packing Heat", 3);
    public static readonly GameTypes RandoCardrissian = new("Rando Cardrissian", 4);
    public static readonly GameTypes GodIsDead = new("God is Dead", 5);
    public static readonly GameTypes SurvivalOfTheFittest = new("Survival of the Fittest",6);
    public static readonly GameTypes SeriousBusiness = new("Serious Business", 7);
    public static readonly GameTypes NeverHaveIEver = new("Never Have I Ever", 8);
}