namespace TheOmenDen.CrowsAgainstHumility.Core.DTO.Messages.Domain;
public sealed class MessageReceivedEventArgs: EventArgs
{
    public MessageReceivedEventArgs(Message message)
    {
        ArgumentNullException.ThrowIfNull(message);
        Message = message;
    }

    public Message Message { get; private set; }
}
