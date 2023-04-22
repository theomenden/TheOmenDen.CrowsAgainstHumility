
using System.Text.Json;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Nodes;

[Serializable]
public record NodeMessage(NodeMessageTypes MessageType, String? SenderNodeId, String? RecipientNodeId);

[Serializable]
public sealed record NodeMessage<T>(NodeMessageTypes MessageType, String? SenderNodeId, String? RecipientNodeId, T Data): NodeMessage(MessageType, SenderNodeId, RecipientNodeId)
{
    public byte[] DataToByteArray() => JsonSerializer.SerializeToUtf8Bytes(Data);
}
