using Ardalis.SmartEnum;

namespace TheOmenDen.CrowsAgainstHumility.Core.Enums;

public abstract class GameRole(string name, int value) : SmartEnum<GameRole>(name, value)
{
    public static readonly GameRole Player = new PlayerRole();
    public static readonly GameRole Judge = new JudgeRole();

    private sealed class PlayerRole() : GameRole("Player", 1);

    private sealed class JudgeRole() : GameRole("Judge", 2);
}