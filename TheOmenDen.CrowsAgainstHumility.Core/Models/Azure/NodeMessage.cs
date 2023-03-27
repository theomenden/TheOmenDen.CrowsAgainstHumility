using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models.Azure;
public class NodeMessage
{
    public NodeMessage(NodeMessageTypes messageType) => MessageType = messageType;
    
    public NodeMessageTypes MessageType { get; private set; }
    public string? SenderNodeId { get; set; }
    public string? RecipientNodeId { get; set; }
    public object? Data { get; set; }
}
