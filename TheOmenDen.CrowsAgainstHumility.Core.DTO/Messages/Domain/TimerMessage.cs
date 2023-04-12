using TheOmenDen.CrowsAgainstHumility.Core.DTO.Serializations;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.DTO.Messages.Domain;
public sealed class TimerMessage : Message
{
    #region Constructors
    public TimerMessage(MessageTypes type, DateTime endedAt)
        : base(type)
    {
        EndedAt = endedAt;
    }

    internal TimerMessage(MessageData messageData) : base(messageData)
    {
        EndedAt = DateTime.SpecifyKind(messageData.EndedAt!.Value, DateTimeKind.Utc);
    }
    #endregion
    #region Properties
    public DateTime EndedAt { get; }
    #endregion
    #region Overrides
    protected internal override MessageData GetData()
    {
        var result = base.GetData();
        result.EndedAt = EndedAt;
        return result;
    }
    #endregion
}
