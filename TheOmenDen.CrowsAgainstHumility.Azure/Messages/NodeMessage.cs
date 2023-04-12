using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Azure.Messages;
public sealed class NodeMessage
{
    public NodeMessage(NodeMessageTypes messageType) => MessageType = messageType;

    public NodeMessageTypes MessageType { get; private set; }
    public string? SenderNodeId { get; set; }
    public string? RecipientNodeId { get; set; }
    public object? Data { get; set; }
}
