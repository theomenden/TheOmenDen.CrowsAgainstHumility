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
