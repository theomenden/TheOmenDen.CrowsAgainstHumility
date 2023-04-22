
using System.Text.Json;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Nodes;
[Serializable]
public abstract record NodeMessage<T>(NodeMessageTypes MessageType, String? SenderNodeId, String? RecipientNodeId, T Data)
{
    public virtual byte[] DataToByteArray() => JsonSerializer.SerializeToUtf8Bytes(Data);
}

[Serializable]
public sealed record ObjectNodeMessage(NodeMessageTypes MessageType, String? SenderNodeId, String? RecipientNodeId, object? Data) : NodeMessage<object?>(MessageType, SenderNodeId, RecipientNodeId, Data);
