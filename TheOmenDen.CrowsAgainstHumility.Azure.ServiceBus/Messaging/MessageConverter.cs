using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Messaging.ServiceBus;
using TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Messaging.Messages;
using TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Nodes;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Messaging;
internal sealed class MessageConverter : IMessageConverter
{
    internal const string RecipientId = nameof(RecipientId);
    internal const string SenderId = nameof(SenderId);

    private const string MessageType = nameof(MessageType);
    private const string MessageSubType = nameof(MessageSubType);
    private static readonly BinaryData _emptyBinaryData = new(Array.Empty<byte>());
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals };

    public async ValueTask<ServiceBusMessage> ConvertToServiceBusMessageAsync(NodeMessage message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(nameof(message));
        cancellationToken.ThrowIfCancellationRequested();

        var messageBody = message.Data is not null
                ? await ConvertToMessageBodyAsync(message.DataToByteArray())
                : _emptyBinaryData;


        var resolvedServiceMessage = new ServiceBusMessage(messageBody)
        {
            ApplicationProperties =
            {
                [MessageType] = message.MessageType.Name,
                [SenderId] = message.SenderNodeId,
                [RecipientId] = message.RecipientNodeId
            }
        };

        if (message.Data is not null)
        {
            resolvedServiceMessage.ApplicationProperties[MessageSubType] = message.Data.GetType().Name;
        }

        return resolvedServiceMessage;
    }

    public async ValueTask<NodeMessage<LobbyMessage>> ConvertToNodeMessageAsync(ServiceBusReceivedMessage message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);
        cancellationToken.ThrowIfCancellationRequested();

        var messageType = NodeMessageTypes.Parse((string)message.ApplicationProperties[MessageType], true);

        var messageSubType = String.Empty;

        var senderId = (string)message.ApplicationProperties[SenderId];
        var recipientId = (string)message.ApplicationProperties[RecipientId];
        
        if (message.ApplicationProperties.TryGetValue(MessageSubType, out var messageSubTypeObject))
        {
            messageSubType = (string)messageSubTypeObject;
        }

        if (String.Equals(messageSubType, nameof(LobbyMemberMessage), StringComparison.OrdinalIgnoreCase))
        {
            return new NodeMessage<LobbyMessage>(
                messageType, 
                senderId, 
                recipientId,
                await ConvertFromMessageBodyAsync<LobbyMemberMessage>(message.Body, cancellationToken));
        }

        if (String.Equals(messageSubType, nameof(LobbyMemberWhiteCardMessage), StringComparison.OrdinalIgnoreCase))
        {
            return new NodeMessage<LobbyMessage>(
                messageType,
                senderId,
                recipientId,
                await ConvertFromMessageBodyAsync<LobbyMemberWhiteCardMessage>(message.Body, cancellationToken));
        }

        if (String.Equals(messageSubType, nameof(LobbyWhiteCardPlayedMessage), StringComparison.OrdinalIgnoreCase))
        {
            return new NodeMessage<LobbyMessage>(
                messageType,
                senderId,
                recipientId,
                await ConvertFromMessageBodyAsync<LobbyWhiteCardPlayedMessage>(message.Body, cancellationToken));
        }

        if (String.Equals(messageSubType, nameof(LobbyRoundTimerMessage), StringComparison.OrdinalIgnoreCase))
        {
            return new NodeMessage<LobbyMessage>(
                messageType,
                senderId,
                recipientId,
                await ConvertFromMessageBodyAsync<LobbyRoundTimerMessage>(message.Body, cancellationToken));
        }

        return new NodeMessage<LobbyMessage>(
            messageType,
            senderId,
            recipientId,
            await ConvertFromMessageBodyAsync<LobbyMessage>(message.Body, cancellationToken));
    }

    private static async Task<BinaryData> ConvertToMessageBodyAsync(byte[] data)
    {
        using var dataStream = new MemoryStream();
        await using var deflateStream = new DeflateStream(dataStream, CompressionMode.Compress, true);

        await deflateStream.WriteAsync(data);
        await deflateStream.FlushAsync();

        return BinaryData.FromBytes(dataStream.ToArray());
    }

    private static ValueTask<T?> ConvertFromMessageBodyAsync<T>(BinaryData body, CancellationToken cancellationToken = default) => JsonSerializer.DeserializeAsync<T?>(body.ToStream(), _jsonSerializerOptions, cancellationToken);

    private static async ValueTask<byte[]> ConvertFromMessageBodyAsync(BinaryData body, CancellationToken cancellationToken = default)
    {

        await using var dataStream = body.ToStream();
        await using var deflateSteam = new DeflateStream(dataStream, CompressionMode.Decompress);
        using var memoryStream = new MemoryStream();

        await deflateSteam.CopyToAsync(memoryStream, cancellationToken);

        return memoryStream?.ToArray() ?? Array.Empty<byte>();
    }
}
