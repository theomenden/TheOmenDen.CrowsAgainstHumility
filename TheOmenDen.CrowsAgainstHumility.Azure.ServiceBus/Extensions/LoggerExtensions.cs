using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Extensions;
internal static class LoggerExtensions
{
    #region Constants
    private const int BaseEventId = 1500;
    #endregion
    #region Private Actions
    private static readonly Action<ILogger, string, Exception?> _crowGameAzureNodeStarting = LoggerMessage.Define<string>(
        LogLevel.Information,
        new EventId(BaseEventId + 1, nameof(CrowGameAzureNodeStarting)),
        "Crow Game Azure Node \"{NodeId}\" is starting."
        );
    private static readonly Action<ILogger, string, Exception?> _crowGameAzureNodeStopping = LoggerMessage.Define<string>(
        LogLevel.Information,
        new EventId(BaseEventId + 2, nameof(CrowGameAzureNodeStopping)),
        "Crow Game Azure Node \"{NodeId}\" is stopping."
        );
    private static readonly Action<ILogger, string, string, Exception?> _errorCreateLobbyNodeMessage = LoggerMessage.Define<string, string>(
        LogLevel.Error,
        new EventId(BaseEventId + 3, nameof(ErrorCreateLobbyNodeMessage)),
        "Error creating LobbyCreated node message for team \"{LobbyName}\" (NodeID: {NodeId})"
        );
    private static readonly Action<ILogger, string, string?, string?, NodeMessageTypes, Exception?> _nodeMessageSent = LoggerMessage.Define<string, string?, string?, NodeMessageTypes>(
        LogLevel.Information,
        new EventId(BaseEventId + 4, nameof(NodeMessageSent)),
        "Crow Game Azure Node message sent (NodeID: {NodeID}, Sender: {Sender}, Recipient: {Recipient}, Type: {Type})"
        );
    private static readonly Action<ILogger, string, string?, string?, NodeMessageTypes, Exception?> _errorSendingNodeMessage = LoggerMessage.Define<string, string?, string?, NodeMessageTypes>(
            LogLevel.Error,
            new EventId(BaseEventId + 5, nameof(ErrorSendingNodeMessage)),
            "Error sending Crow Game Azure Node message (NodeID: {NodeId}, Sender: {Sender}, Recipient: {Recipient}, Type: {Type})"
        );
    private static readonly Action<ILogger, string, string?, string?, NodeMessageTypes, string, Exception?> _lobbyCreatedNodeMessageReceived = LoggerMessage.Define<string, string?, string?, NodeMessageTypes, string>(
            LogLevel.Information,
            new EventId(BaseEventId + 6, nameof(LobbyCreatedNodeMessageReceived)),
            "Crow Game Azure Node message received (NodeID: {NodeId}, Sender: {Sender}, Recipient: {Recipient}, Type: {Type})"
        );
    private static readonly Action<ILogger, string, string?, string?, NodeMessageTypes, Exception?> _errorCreatingLobbyNodeMessage = LoggerMessage.Define<string, string?, String?, NodeMessageTypes>(
            LogLevel.Error,
            new EventId(BaseEventId + 7, nameof(ErrorCreatingLobbyNodeMessage)),
            "Error processing Crow Game Azure Node message (NodeID: {NodeId}, Sender: {Sender}, Recipient: {Recipient}, Type: {Type})"
        );
    private static readonly Action<ILogger, string, string?, string?, NodeMessageTypes, string?, MessageTypes, Exception?> _lobbyNodeMessageReceived = LoggerMessage.Define<string, string?, string?, NodeMessageTypes, string?, MessageTypes>(
                LogLevel.Information,
                new EventId(BaseEventId + 8, nameof(LobbyNodeMessageReceived)),
                "Crow Game Azure Node message received (NodeID: {NodeId}, Sender: {Sender}, Recipient: {Recipient}, Type: {Type}, Lobby: {Lobby}, MessageType: {MessageType})"
            );
    private static readonly Action<ILogger, string, string?, string?, NodeMessageTypes, string?, MessageTypes, Exception?> _errorProcessingLobbyNodeMessage = LoggerMessage.Define<string, string?, String?, NodeMessageTypes, String?, MessageTypes>(
                LogLevel.Error,
                new EventId(BaseEventId + 9, nameof(ErrorProcessingLobbyNodeMessage)),
                "Error processing Crow Game Azure Node message (NodeID: {NodeId}, Sender: {Sender}, Recipient: {Recipient}, Type: {Type}, Lobby: {Lobby}, Message type: {MessageType})"
            );
    private static readonly Action<ILogger, string, string?, string?, NodeMessageTypes, Exception?> _nodeMessageReceived = LoggerMessage.Define<string, string?, string?, NodeMessageTypes>(
            LogLevel.Information,
            new EventId(BaseEventId + 10, nameof(NodeMessageReceived)),
            "Crow Game Azure Node message received (NodeID: {NodeId}, Sender: {Sender}, Recipient: {Recipient}, Type: {Type})"
        );
    private static readonly Action<ILogger, string, Exception?> _retryRequestLobbyList = LoggerMessage.Define<string>(
        LogLevel.Warning,
        new EventId(BaseEventId + 11, nameof(RetryRequestLobbyList)),
        "Retry to request Lobby list for Node \"{NodeId}\"."
        );
    private static readonly Action<ILogger, string, Exception?> _crowGameAzureNodeInitialized = LoggerMessage.Define<string>(
        LogLevel.Information,
        new EventId(BaseEventId + 12, nameof(CrowGameAzureNodeInitialized)),
        "Crow Game Azure Node \"{NodeId}\" is initialized"
        );
    #endregion
    #region Public Extensions
    public static void CrowGameAzureNodeStarting(this ILogger logger, string nodeId) => _crowGameAzureNodeStarting(logger, nodeId, null);
    public static void CrowGameAzureNodeStopping(this ILogger logger, string nodeId) => _crowGameAzureNodeStopping(logger, nodeId, null);
    public static void ErrorCreateLobbyNodeMessage(this ILogger logger, Exception exception, string nodeId, string lobbyName) => _errorCreateLobbyNodeMessage(logger, lobbyName, nodeId, exception);
    public static void NodeMessageSent(this ILogger logger, string nodeId, string? senderNodeId, string? recipientNodeId, NodeMessageTypes type) => _nodeMessageSent(logger, nodeId, senderNodeId, recipientNodeId, type, null);
    public static void ErrorSendingNodeMessage(this ILogger logger, Exception exception, string nodeId, string? senderNodeId, string? recipientNodeId, NodeMessageTypes type) => _errorSendingNodeMessage(logger, nodeId, senderNodeId, recipientNodeId, type, exception);
    public static void LobbyCreatedNodeMessageReceived(this ILogger logger, string nodeId, string? senderNodeId, string? recipientNodeId, NodeMessageTypes type, string lobby) => _lobbyCreatedNodeMessageReceived(logger, nodeId, senderNodeId, recipientNodeId, type, lobby, null);
    public static void ErrorCreatingLobbyNodeMessage(this ILogger logger, Exception exception, string nodeId, string? senderNodeId, string? recipientNodeId, NodeMessageTypes type) => _errorCreatingLobbyNodeMessage(logger, nodeId, senderNodeId, recipientNodeId, type, exception);
    public static void LobbyNodeMessageReceived(this ILogger logger, string nodeId, string? senderNodeId, string? recipientNodeId, NodeMessageTypes type, string? lobby, MessageTypes messageType) => _lobbyNodeMessageReceived(logger, nodeId, senderNodeId, recipientNodeId, type, lobby, messageType, null);
    public static void ErrorProcessingLobbyNodeMessage(this ILogger logger, Exception exception, string nodeId, string? senderNodeId, string? recipientNodeId, NodeMessageTypes type, string? lobby, MessageTypes messageTypes) => _errorProcessingLobbyNodeMessage(logger, nodeId, senderNodeId, recipientNodeId, type, lobby, messageTypes, exception);
    public static void NodeMessageReceived(this ILogger logger, string nodeId, string? senderNodeId, string? recipientNodeId, NodeMessageTypes type) => _nodeMessageReceived(logger, nodeId, senderNodeId, recipientNodeId, type, null);
    public static void RetryRequestLobbyList(this ILogger logger, string nodeId) => _retryRequestLobbyList(logger, nodeId, null);
    public static void CrowGameAzureNodeInitialized(this ILogger logger, string nodeId) => _crowGameAzureNodeInitialized(logger, nodeId, null);
    #endregion
}

internal static class ServiceBusLoggerExtensions
{
    #region Constants
    private const int BaseEventId = 1600;
    #endregion
    #region Private Actions
    private static readonly Action<ILogger, Exception?> _sendMessage = LoggerMessage.Define(
        LogLevel.Debug,
        new EventId(BaseEventId + 1, nameof(SendMessage)),
        "Message sent to Azure Service Bus.");
    private static readonly Action<ILogger, Exception?> _errorSendingMessage = LoggerMessage.Define(
        LogLevel.Error,
        new EventId(BaseEventId + 2, nameof(ErrorSendingMessage)),
        "Error sending message to Azure Service Bus.");
    private static readonly Action<ILogger, string, string, Exception?> _subscriptionCreated = LoggerMessage.Define<string, string>(
        LogLevel.Debug,
        new EventId(BaseEventId + 3, nameof(SubscriptionCreated)),
            "Service Bus Subscription was created (Topic: {Topic}, NodeID: {NodeId})");
    private static readonly Action<ILogger, string?, string?, Exception?> _subscriptionDeleted = LoggerMessage.Define<string?, string?>(
            LogLevel.Debug,
            new EventId(BaseEventId + 4, nameof(SubscriptionDeleted)),
            "Service Bus Subscription was deleted (Topic: {Topic}, NodeID: {NodeId})");
    private static readonly Action<ILogger, string?, string?, string?, Exception?> _messageReceived = LoggerMessage.Define<string?, string?, string?>(
            LogLevel.Debug,
            new EventId(BaseEventId + 5, nameof(MessageReceived)),
            "Service Bus Message was received (Topic: {Topic}, NodeID: {NodeId}, MessageID: {MessageId})"
        );
    private static readonly Action<ILogger, string?, string?, string?, Exception?> _messageProcessed = LoggerMessage.Define<string?, string?, string?>(
        LogLevel.Information,
        new EventId(BaseEventId + 6, nameof(MessageProcessed)),
        "Service Bus Message was processed (Topic: {Topic}, NodeID: {NodeId}, MessageID: {MessageId})");
    private static readonly Action<ILogger, string?, string?, string?, Exception?> _errorProcessingMessage = LoggerMessage.Define<string?, string?, string?>(
        LogLevel.Error,
        new EventId(BaseEventId + 7, nameof(ErrorProcessingMessage)),
        "Service Bus Message processing failed (Topic: {Topic}, NodeID: {NodeId}, MessageID: {MessageId})"
        );
    private static readonly Action<ILogger, string?, string?, ServiceBusErrorSource, Exception?> _errorProcessing = LoggerMessage.Define<string?, string?, ServiceBusErrorSource>(
        LogLevel.Error,
        new EventId(BaseEventId + 8, nameof(ErrorProcessing)),
        "Service Bus Processing failed (Topic {Topic}, NodeID: {NodeId}, MessageID: {MessageId}"
        );
    private static readonly Action<ILogger, string?, Exception?> _errorSubscriptionsMaintenance = LoggerMessage.Define<string?>(
        LogLevel.Error,
        new EventId(BaseEventId + 9, nameof(ErrorSubscriptionsMaintenance)),
        "Service Bus Subscriptions maintenance failed for NodeID: {NodeId}"
        );
    private static readonly Action<ILogger, string?, string?, string?, Exception?> _subscriptionAliveMessageReceived = LoggerMessage.Define<string?, string?, string?>(
        LogLevel.Debug,
        new EventId(BaseEventId + 10, nameof(SubscriptionAliveMessageReceived)),
        "Service Bus Subscription is alive:(Topic: {Topic}, NodeID: {NodeId}, SubscriptionID: {SubscriptionId})"
        );
    private static readonly Action<ILogger, string?, Exception?> _subscriptionAliveSent = LoggerMessage.Define<string?>(
        LogLevel.Debug,
        new EventId(BaseEventId + 11, nameof(SubscriptionAliveSent)),
        "Notification sent indicating NodeID: \"{NodeId}\" is alive."
        );
    private static readonly Action<ILogger, string?, string?, Exception?> _inactiveSubscriptionDeleted =
        LoggerMessage.Define<string?, String?>(
            LogLevel.Debug,
            new EventId(BaseEventId + 12, nameof(InactiveSubscriptionDeleted)),
            "Service Bus Subscription \"{SubscriptionId}\" was deleted due to inactivity by Node \"{NodeId}\"."
            );
    private static readonly Action<ILogger, string?, string?, string, Exception?> _subscriptionDeleteFailed = LoggerMessage.Define<string?, string?, string>(
        LogLevel.Warning,
        new EventId(BaseEventId + 13, nameof(SubscriptionDeleteFailed)),
        "Deletion of Service Bus Subscription (Topic: {Topic}, NodeID: {SubscriptionId}) Failed with Error: {Error}"
        );
    #endregion
    #region Public Extensions
    public static void SendMessage(this ILogger logger) => _sendMessage(logger, null);
    public static void SubscriptionCreated(this ILogger logger, string topicName, string nodeId) => _subscriptionCreated(logger, topicName, nodeId, null);
    public static void ErrorSendingMessage(this ILogger logger, Exception exception) => _errorSendingMessage(logger, exception);
    public static void SubscriptionDeleted(this ILogger logger, string? topicName, string? nodeId) => _subscriptionDeleted(logger, topicName, nodeId, null);
    public static void MessageReceived(this ILogger logger, string? topicName, string? nodeId, string? messageId) => _messageReceived(logger, topicName, nodeId, messageId, null);
    public static void MessageProcessed(this ILogger logger, string? topicName, string? nodeId, string? messageId) => _messageProcessed(logger, topicName, nodeId, messageId, null);
    public static void ErrorProcessingMessage(this ILogger logger, Exception exception, string? topicName, string? nodeId, string? messageId) => _errorProcessingMessage(logger, topicName, nodeId, messageId, exception);
    public static void ErrorProcessing(this ILogger logger, Exception exception, string? topicName, string? nodeId, ServiceBusErrorSource errorSource) => _errorProcessing(logger, topicName, nodeId, errorSource, exception);
    public static void ErrorSubscriptionsMaintenance(this ILogger logger, Exception exception, string? nodeId) => _errorSubscriptionsMaintenance(logger, nodeId, exception);
    public static void SubscriptionAliveMessageReceived(this ILogger logger, string? topicName, string? nodeId, string? subscriptionId) => _subscriptionAliveMessageReceived(logger, topicName, nodeId, subscriptionId, null);
    public static void SubscriptionAliveSent(this ILogger logger, string? nodeId) => _subscriptionAliveSent(logger, nodeId, null);
    public static void InactiveSubscriptionDeleted(this ILogger logger, string? nodeId, string? subscriptionId) => _inactiveSubscriptionDeleted(logger, subscriptionId, nodeId, null);
    public static void SubscriptionDeleteFailed(this ILogger logger, Exception exception, string? topicName, string? subscriptionId) => _subscriptionDeleteFailed(logger, topicName, subscriptionId, exception.Message, null);
    #endregion
}
