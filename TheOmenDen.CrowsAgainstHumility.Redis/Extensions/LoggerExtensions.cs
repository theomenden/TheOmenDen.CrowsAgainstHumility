using Microsoft.Extensions.Logging;
namespace TheOmenDen.CrowsAgainstHumility.Redis.Extensions;
public static class LoggerExtensions
{
    #region Constants
    private const int BaseEventId = 1700;
    #endregion
    #region Private static Actions
    private static readonly Action<ILogger, Exception?> _sendMessage = LoggerMessage.Define(LogLevel.Debug,
        new EventId(BaseEventId + 1, nameof(SendMessage)), "Message sent to Redis.");

    private static readonly Action<ILogger, Exception?> _errorSendMessage = LoggerMessage.Define(LogLevel.Debug,
        new EventId(BaseEventId + 2, nameof(ErrorSendMessage)), "Error sending message to Redis.");

    private static readonly Action<ILogger, string, string, Exception?> _subscriptionCreated = LoggerMessage.Define<string, string>(LogLevel.Debug,
        new EventId(BaseEventId + 3, nameof(SubscriptionCreated)), "Redis subscription was created (Channel: {Channel}, NodeID {NodeId})");

    private static readonly Action<ILogger, string?, string?, string?, Exception?> _messageReceived = LoggerMessage.Define<string?, string?, string?>(LogLevel.Debug,
        new EventId(BaseEventId + 5, nameof(MessageReceived)), "Redis message was received (Channel: {Channel}, NodeID: {NodeId}, MessageID: {MessageId})");

    private static readonly Action<ILogger, string?, string?, string?, Exception?> _messageProcessed = LoggerMessage.Define<string?, string?, string?>(LogLevel.Debug,
        new EventId(BaseEventId + 6, nameof(MessageProcessed)), "Redis message was processed (Channel: {Channel}, NodeID: {NodeId}, MessageID {MessageId}");

    private static readonly Action<ILogger, string?, string?, string?, Exception?> _errorProccessMessage = LoggerMessage.Define<string?, string?, string?>(LogLevel.Debug,
        new EventId(BaseEventId + 7, nameof(ErrorProcessMessage)), "Redis message processing failed (Channel: {Channel}, NodeID: {NodeId}, MessageID: {MessageId})");
    #endregion
    #region Extension Methods
    public static void SendMessage(this ILogger logger) => _sendMessage(logger, null);
    public static void ErrorSendMessage(this ILogger logger, Exception exception)
        => _errorSendMessage(logger, exception);
    public static void SubscriptionCreated(this ILogger logger, string channel, string nodeId)
        => _subscriptionCreated(logger, channel, nodeId, null);
    public static void MessageReceived(this ILogger logger, string? channel, string? nodeId, string? messageId)
        => _messageReceived(logger, channel, nodeId, messageId, null);
    public static void MessageProcessed(this ILogger logger, string? channel, string? nodeId, string? messageId)
        => _messageProcessed(logger, channel, nodeId, messageId, null);
    public static void ErrorProcessMessage(this ILogger logger, Exception exception, string? channel, string? nodeId, string? messageId)
        => _errorProccessMessage(logger, channel, nodeId, messageId, exception);
    #endregion
}