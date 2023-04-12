using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.DTO.Messages.Domain;
public class Message
{
    #region Constructors
    public Message(MessageTypes type)
    {
        MessageType = type;
    }

    public Message(MessageTypes type, long id)
    {
        MessageType = type;
        Id = id;
    }

    internal Message(Serializations.MessageData messageData)
    {
        MessageType = messageData.MessageType;
        Id = messageData.Id;
    }
    #endregion
    #region Properties
    public MessageTypes MessageType { get; private set; }
    public long Id { get; internal set; }
    #endregion
    #region Protected Methods
    protected internal virtual Serializations.MessageData GetData()
        => new()
        {
            MessageType = MessageType,
            Id = Id
        };
    #endregion
}
