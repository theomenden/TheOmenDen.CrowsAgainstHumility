namespace TheOmenDen.CrowsAgainstHumility.Core.Models;
public sealed class PlayerScore
{
    public PlayerScore(PlayerDto player, Int32? score)
    {
        Player = player;
        Score = score ?? 2;
    }

    public Int32 Score { get; set; }

    public PlayerDto Player { get; set; }
}
