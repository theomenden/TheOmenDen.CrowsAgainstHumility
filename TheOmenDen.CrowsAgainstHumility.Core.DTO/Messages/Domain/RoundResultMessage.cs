using TheOmenDen.CrowsAgainstHumility.Core.DTO.Results.Domain;
using TheOmenDen.CrowsAgainstHumility.Core.DTO.Serializations;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.DTO.Messages.Domain;
public sealed class RoundResultMessage : Message
{
    #region Constructors
    public RoundResultMessage(MessageTypes type, RoundResult roundResult)
    : base(type)
    {
        ArgumentNullException.ThrowIfNull(roundResult);
        RoundResult = roundResult;
    }

    internal RoundResultMessage(Serializations.MessageData messageData, RoundResult roundResult)
        : base(messageData)
    {
        ArgumentNullException.ThrowIfNull(roundResult);
        RoundResult = roundResult;
    }
    #endregion
    #region Properties
    public RoundResult RoundResult { get; }
    #endregion
    #region Overrides
    protected internal override MessageData GetData()
    {
        var result = base.GetData();
        result.PlayedCardResult = RoundResult.GetData();
        return result;
    }
    #endregion
}
