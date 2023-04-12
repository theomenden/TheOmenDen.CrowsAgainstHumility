using TheOmenDen.CrowsAgainstHumility.Core.Messages;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models.EventArgs;
public sealed class MessageReceivedEventArgs : System.EventArgs
{
    public MessageReceivedEventArgs(Message message)
    {
        Message = message;
    }
    
    public Message Message { get; }
}
