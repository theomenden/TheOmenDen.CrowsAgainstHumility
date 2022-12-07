using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.EventArgs;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models.EventArgs;
public sealed class WhiteCardPlayedEventArgs: IWhiteCardPlayedEventArgs
{
    public WhiteCardPlayedEventArgs(WhiteCard playedCard, bool shouldBeBackToPool)
    {
        PlayedWhiteCard = playedCard; 
        ShouldBeBackInPool = shouldBeBackToPool;
    }

    public WhiteCard PlayedWhiteCard { get; set; }

    public Boolean ShouldBeBackInPool { get; set; }
}
