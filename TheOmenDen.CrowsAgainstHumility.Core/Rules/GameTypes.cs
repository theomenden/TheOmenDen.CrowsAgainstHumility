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
    public static readonly GameTypes WheatonsLaw = new("Wheaton's Law", 9);
    public static readonly GameTypes ChubbyBunny = new("Chubby Bunny", 10);
    public static readonly GameTypes FreakyFriday = new("Freaky Friday", 11);
    public static readonly GameTypes HailToTheCheif = new("Hail to the Chief", 12);
    public static readonly GameTypes HardMode = new("Hard Mode", 13);
    public static readonly GameTypes Meritocracy = new("Meritocracy", 14);
    public static readonly GameTypes RaceToTheMoon = new("Race To The Moon", 15);
    public static readonly GameTypes RussianRoulette = new("Russian Roulette", 16);
    public static readonly GameTypes SmokeOpiumAndPlay = new("Smoke Opium and Play Cards Against Humanity", 17);
    public static readonly GameTypes TieBreaker = new("Tie Breaker", 18);
    public static readonly GameTypes WaitForGodot = new("Wait for Godot", 19);
    public static readonly GameTypes SmoothOperator = new("Smooth Operator", 20);
    public static readonly GameTypes DontPlay = new("Don't Play Cards Against Humanity", 21);
}