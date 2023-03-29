using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Azure.Extensions;
internal static class LoggerExtensions
{
    #region Constants
    private const int BaseEventId = 1500;
    #endregion
    #region Private Action Delegates
    private static readonly Action<ILogger, string, Exception?> _crowGameAzureNodeStarting = LoggerMessage<string>(
        LogLevel.Information,
        new EventId(BaseEventId + 1, nameof(CrowGameAzureNodeStarting)),
            "Crows Against Humility Azure Node \"{NodeId}\" is starting.");

    private static readonly Action<ILogger, string, Exception?> _crowGameAzureNodeStopping =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(BaseEventId + 2, nameof(CrowGameAzureNodeStopping)),
            "Crows Against Humility Azure Node \"{NodeId}\" is stopping.");

    private static readonly Action<ILogger, string, string, Exception?> _errorCreatePlayerListNodeMessage =
        LoggerMessage.Define<string, string>(
            LogLevel.Error,
            new EventId(BaseEventId + 3, nameof(ErrorCreatePlayerListNodeMessage)),
            "Error creating PlayerListCreated node message for the list\"{TeamName}\" (NodeID: {NodeId})");
    private static readonly Action<ILogger, string, string?, string?, NodeMessageTypes, Exception?> _nodeMessageSent =
        LoggerMessage.Define<string, string?, string?, NodeMessageTypes>(
    LogLevel.Information,
    new EventId(BaseEventId + 4, nameof(NodeMessageSent)),
    "Crows Against Humility Azure Node message sent (NodeID: {NodeId}, Sender: {Sender}, Recipient: {Recipient}, Type: {Type}");

    private static readonly Action<ILogger, string, string? string?, NodeMessageTypes, Exception?> _errorSendingNodeMessage =
        LoggerMessage.Define<string, string?, string?, NodeMessageTypes>(
            LogLevel.Error,
            new EventId(BaseEventId + 5, nameof(ErrorSendingNodeMessage)),
            "Error sending Crows Against Humility Node message (NodeID: {NodeId}, Sender: {Sender}, Recipient: {Recipient}, Type: {Type})"
        );
    private static readonly Action<ILogger, string, Exception?>
    private static readonly Action<ILogger, string, Exception?>
    private static readonly Action<ILogger, string,string?, string?, NodeMessageTypes, string?, MessageType, Exception?>

    private static readonly Action<ILogger, string, string?, string?, NodeMessageTypes, string?, MessageType, Exception?> _errorProcessingPlayerListNodeMessage =
        LoggerMessage.Define<string, string?, string?, NodeMessageTypes, String?, MessageType>
        (
            LogLevel.Error,
            new EventId(BaseEventId + 9, nameof(ErrorProcessingPlayerListNodeMessage)),
            "Error processing Crows Against Humility Azure Node message (NodeID: {NodeId}, Sender: {Sender}, Recipient: {Recipient}, PlayerList: {PlayerList}, Message type: {MessageType})");

    private static readonly Action<ILogger, string, string?, string?, NodeMessageTypes, Exception?>
        _nodeMessageReceived = LoggerMessage.Define<string, string?, String?, NodeMessageTypes>(
                LogLevel.Information,
                new EventId(BaseEventId + 10, nameof(NodeMessageReceived)),
                "Crows Against Humility Node message received (NodeID: {NodeId}, Sender: {Sender}, Recipient: {Recipient}, Type: {Type})"
            );

    private static readonly Action<ILogger, string, Exception?> _retryRequestPlayerList = LoggerMessage.Define<string>(
            LogLevel.Warning,
            new EventId(BaseEventId + 11, nameof(RetryRequestPlayerList)),
            "Retry to request Player list for Node \"{NodeId}\".")
        );

    private static readonly Action<ILogger, string, Exception?> _crowsAgainstHumilityAzureNodeInitialized =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(BaseEventId + 12, nameof(CrowsAgainstHumilityAzureNodeInitialized)),
            "Crows Against Humility Azure Node \"{NodeId}\" is initialized"
        );
    #endregion

    #region Public Methods
    public static void ErrorProcessingPlayerListNodeMessage(this ILogger logger, Exception exception, string nodeId, string? senderNodeId, string? recipientNodeId, NodeMessageTypes type, string? playerList, MessageType messageType)
    public static void NodeMessageReceived(this ILogger logger, string nodeId, string? senderNodeId, string? recipientNodeId, NodeMessageTypes type)
    => _nodeMessageReceived(logger, nodeId, senderNodeId, type, null);
    public static void RetryRequestPlayerList(this ILogger logger, string nodeId)
    => _retryRequestPlayerList(logger, nodeId, null);
    public static void CrowsAgainstHumilityAzureNodeInitialized(this ILogger logger, string nodeId)
    => _crowsAgainstHumilityAzureNodeInitialized(logger, nodeId, null);
    #endregion
}
