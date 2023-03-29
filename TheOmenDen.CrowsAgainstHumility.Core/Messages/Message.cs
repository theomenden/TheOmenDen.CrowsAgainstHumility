using System.Runtime.Serialization;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.Messages;
public class Message
{
    public Message(MessageTypes type)
    {
        MessageType = type;
    }

    public Message(MessageTypes type, long id)
    {
        MessageType = type;
        Id = id;
    }

    internal Message(Serialization.MessageData messageData) =>
        (MessageType = messageData.MessageType, Id = messageData.Id);

    public MessageTypes MessageType { get; private set; }
    public long Id { get; internal set; }

    protected internal virtual Serialization.MessageData GetData() => new Serialization.MessageData()
    {
        MessageType = MessageType,
        Id = Id
    };
}
