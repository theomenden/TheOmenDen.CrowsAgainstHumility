using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Results;
using TheOmenDen.CrowsAgainstHumility.Core.Serialization;

namespace TheOmenDen.CrowsAgainstHumility.Core.Messages;
public class RoundResultMessage: Message
{
    public RoundResultMessage(MessageTypes type, RoundResult roundResult) : base(type)
    {
        RoundResult = roundResult ?? throw new ArgumentNullException(nameof(roundResult));
    }
    
    internal RoundResultMessage(MessageData messageData, RoundResult roundResult) : base(messageData)
    {
        RoundResult = roundResult ?? throw new ArgumentNullException(nameof(roundResult));
    }

    public RoundResult RoundResult { get; }

    protected internal override Serialization.MessageData GetData()
    {
        var result = base.GetData();
        result.PlayedResult = RoundResult.GetData();
        return result;
    }
}
