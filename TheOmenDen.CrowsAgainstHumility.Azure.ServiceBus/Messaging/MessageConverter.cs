using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Messaging.ServiceBus;
using TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Messaging.Messages;
using TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Nodes;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TwitchLib.PubSub.Models.Responses.Messages.AutomodCaughtMessage;

namespace TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Messaging;
internal sealed class MessageConverter : IMessageConverter
{
    internal const string RecipientId = nameof(RecipientId);
    internal const string SenderId = nameof(SenderId);

    private const string MessageType = nameof(MessageType);
    private const string MessageSubType = nameof(MessageSubType);
    private static readonly BinaryData _emptyBinaryData = new(Array.Empty<byte>());
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals };

    public async ValueTask<ServiceBusMessage> ConvertToServiceBusMessageAsync(ObjectNodeMessage message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        var messageBody = message.Data is not null
            ? await ConvertToMessageBodyAsync(message.DataToByteArray())
            : _emptyBinaryData;

        return new ServiceBusMessage(messageBody)
        {
            ApplicationProperties =
           {
               [MessageType] = message.MessageType.ToString(),
               [MessageSubType] = message.Data?.GetType()?.Name ?? String.Empty,
               [SenderId] = message.SenderNodeId,
               [RecipientId] = message.RecipientNodeId,
           }
        };
    }

    public async ValueTask<ObjectNodeMessage> ConvertToNodeMessageAsync(ServiceBusReceivedMessage message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        var messageType = NodeMessageTypes.Parse((string)message.ApplicationProperties[MessageType], true);
        var messageSubType = String.Empty;

        if (message.ApplicationProperties.TryGetValue(MessageSubType, out var messageSubTypeObject))
        {
            messageSubType = (string)messageSubTypeObject;
        }

        var senderNodeId = (string)message.ApplicationProperties[SenderId];
        var recipientNodeId = (string)message.ApplicationProperties[RecipientId];

        object? data = null;

        messageType
            .When(NodeMessageTypes.PlayerList, NodeMessageTypes.RequestPlayerList)
                .ThenAwait(async () =>
            {
                data = await ConvertFromMessageBodyAsync<string[]>(message.Body, cancellationToken);
            })
            .When(NodeMessageTypes.LobbyCreated, NodeMessageTypes.InitializePlayers)
                .ThenAwait(async () =>
            {
                data = await ConvertFromMessageBodyAsync(message.Body, cancellationToken);
            })
            .When(NodeMessageTypes.PlayerList)
                .ThenAwait(async () =>
            {
                if (String.Equals(messageSubType, nameof(LobbyMemberMessage), StringComparison.OrdinalIgnoreCase))
                {
                    data = await ConvertFromMessageBodyAsync<LobbyMemberMessage>(message.Body, cancellationToken);
                }
                else if (String.Equals(messageSubType, nameof(LobbyMemberWhiteCardMessage), StringComparison.OrdinalIgnoreCase))
                {
                    data = await ConvertFromMessageBodyAsync<LobbyMemberWhiteCardMessage>(message.Body, cancellationToken);
                }
                else if (String.Equals(messageSubType, nameof(LobbyWhiteCardPlayedMessage), StringComparison.OrdinalIgnoreCase))
                {
                    data = await ConvertFromMessageBodyAsync<LobbyWhiteCardPlayedMessage>(message.Body, cancellationToken);
                }
                else if (String.Equals(messageSubType, nameof(LobbyRoundTimerMessage), StringComparison.OrdinalIgnoreCase))
                {
                    data = await ConvertFromMessageBodyAsync<LobbyRoundTimerMessage>(message.Body, cancellationToken);
                }
                else
                {
                    data = await ConvertFromMessageBodyAsync<LobbyMessage>(message.Body, cancellationToken);
                }
            });
        
        return new ObjectNodeMessage(messageType, senderNodeId, recipientNodeId, data);
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
