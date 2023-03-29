using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Serialization;

namespace TheOmenDen.CrowsAgainstHumility.Core.Messages;
public sealed class TimerMessage : Message
{
    public TimerMessage(MessageTypes type, DateTime endTime) : base(type) => EndTime = endTime;

    internal TimerMessage(Serialization.MessageData messageData) : base(messageData) =>
        EndTime = DateTime.SpecifyKind(messageData.EndTime!.Value, DateTimeKind.Utc);

    public DateTime EndTime { get; }

    protected internal override MessageData GetData()
    {
        var result = base.GetData();
        result.EndTime = EndTime;
        return result;
    }
}
