using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using StackExchange.Redis;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Redis;
using TheOmenDen.CrowsAgainstHumility.Core.Models.Azure;

namespace TheOmenDen.CrowsAgainstHumility.Redis.MessageConverters;
internal sealed class RedisMessageConverter : IRedisMessageConverter
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
        PropertyNameCaseInsensitive = true
    };

    public RedisValue ConvertToRedisMessage(NodeMessage message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        using var dataStream = new MemoryStream();
        WriteToStream(message.SenderNodeId, dataStream);
        WriteToStream(message.RecipientNodeId, dataStream);
        dataStream.WriteByte((byte)message.MessageType);
        WriteToStream(message.Data?.GetType().Name, dataStream);
        dataStream.Flush();

        message.MessageType
            .When(NodeMessageTypes.InitializePlayers, NodeMessageTypes.LobbyCreated)
                .Then(() => WriteToStream((byte[])message.Data!, dataStream))
            .DefaultCondition(() =>
            {
                if (message.Data is not null)
                {
                    WriteToStream(message.Data, dataStream);
                }
            });


        return RedisValue.CreateFrom(dataStream);
    }

    public NodeMessage ConvertToNodeMessage(RedisValue message)
    {
        if (message.IsNullOrEmpty)
        {
            throw new ArgumentNullException(nameof(message));
        }

        var data = ((ReadOnlyMemory<byte>)message).Span;
        data = ReadString(data, out var senderNodeId);
        data = ReadString(data, out var recipientNodeId);

        var messageType = data[0];
        data = data[1..];

        var parseResult = NodeMessageTypes.ParseFromValue(messageType);

        if (!NodeMessageTypes.ReadOnlyEnumerationList.Contains(parseResult))
        {
            throw new ArgumentException("Invalid message format.", nameof(message));
        }

        data = ReadString(data, out var messageSubtype);

        var result = new NodeMessage(parseResult)
        {
            SenderNodeId = senderNodeId,
            RecipientNodeId = recipientNodeId
        };

        if (result.MessageType == NodeMessageTypes.InitializePlayers ||
            result.MessageType == NodeMessageTypes.LobbyCreated)
        {
            result.Data = ReadBinary(data);
            return result;
        }

        if (result.MessageType == NodeMessageTypes.PlayerList ||
            result.MessageType == NodeMessageTypes.RequestPlayerList)
        {
            result.Data = ReadObject<string[]>(data);
            return result;
        }

        if (result.MessageType != NodeMessageTypes.PlayerListMessage)
        {
            return result;
        }

        result.Data = DetermineMessageSubType(messageSubtype, data);
        return result;

    }

    public NodeMessage GetMessageHeader(RedisValue message)
    {
        if (message.IsNullOrEmpty)
        {
            throw new ArgumentNullException(nameof(message));
        }

        var data = ((ReadOnlyMemory<byte>)message).Span;

        data = ReadString(data, out var senderNodeId);
        data = ReadString(data, out var recipientNodeId);

        var messageType = NodeMessageTypes.ParseFromValue(data[0]);

        return !NodeMessageTypes.ReadOnlyEnumerationList.Contains(messageType)
            ? throw new ArgumentException("Invalid message format", nameof(message))
            : new NodeMessage(messageType)
            {
                SenderNodeId = senderNodeId,
                RecipientNodeId = recipientNodeId
            };
    }
    #region Private Static Methods
    private static object? DetermineMessageSubType(string? messageSubtype, ReadOnlySpan<byte> data)
    {
        return messageSubtype switch
        {
            { } when String.Equals(messageSubtype, nameof(PlayerListMemberMessage), StringComparison.OrdinalIgnoreCase) => ReadObject<PlayerListMemberMessage>(data),
            { } when String.Equals(messageSubtype, nameof(PlayerWhiteCardSubmittedMessage), StringComparison.OrdinalIgnoreCase) => ReadObject<PlayerWhiteCardSubmittedMessage>(data),
            { } when String.Equals(messageSubtype, nameof(PlayerListBlackCardChosenMessage), StringComparison.OrdinalIgnoreCase) => ReadObject<PlayerListBlackCardChosenMessage>(data),
            { } when String.Equals(messageSubtype, nameof(PlayerListTimerMessage), StringComparison.OrdinalIgnoreCase) => ReadObject<PlayerListTimerMessage>(data),
            _ => ReadObject<PlayerListMessage>(data)
        };
    }
    private static void WriteToStream(object data, Stream stream)
    => JsonSerializer.Serialize(stream, data, data.GetType(), JsonSerializerOptions);
    private static Task WriteToStreamAsync(object data, Stream stream, CancellationToken cancellationToken = default)
    => JsonSerializer.SerializeAsync(stream, data, JsonSerializerOptions, cancellationToken);
    private static async Task WriteToStreamAsync(byte[] data, Stream stream, CancellationToken cancellationToken = default)
    {
        await using var deflateStream = new DeflateStream(stream, CompressionMode.Compress, true);
        await deflateStream.WriteAsync(data, cancellationToken);
        await deflateStream.FlushAsync(cancellationToken);
    }
    private static void WriteToStream(byte[] data, Stream stream)
    {
        using var deflateStream = new DeflateStream(stream, CompressionMode.Compress, true);
        deflateStream.Write(data);
        deflateStream.Flush();
    }
    private static ReadOnlySpan<byte> ReadString(ReadOnlySpan<byte> data, out string? value)
    {
        if (data.IsEmpty)
        {
            throw new ArgumentNullException("Invalid message format.", nameof(data));
        }

        var length = data[0];

        switch (length)
        {
            case 255:
                value = null;
                return data[1..];
            case 0:
                value = string.Empty;
                return data[1..];
            default:
                value = Encoding.UTF8.GetString(data.Slice(1, length));
                return data[(length + 1)..];
        }
    }
    private static T? ReadObject<T>(ReadOnlySpan<byte> data) => JsonSerializer.Deserialize<T>(data, JsonSerializerOptions);
    private static byte[] ReadBinary(ReadOnlySpan<byte> data)
    {
        using var dataStream = new MemoryStream(data.ToArray());
        using var deflateStream = new DeflateStream(dataStream, CompressionMode.Decompress);
        using var memoryStream = new MemoryStream();
        deflateStream.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }
    #endregion
}
