using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.DTO.Models.Cards;

namespace TheOmenDen.CrowsAgainstHumility.Azure.Extensions;
internal static class LoggingExtensions
{
    #region Cosmos Db Logger
    private const int BaseCosmosEventId = 1050;
    #region Private Actions
    private static readonly Action<ILogger, Exception?> _loadLobbyNames = LoggerMessage.Define(
        LogLevel.Debug,
        new EventId(BaseCosmosEventId + 1, nameof(LoadLobbyNames)),
        "Loading Lobby Names.");
    private static readonly Action<ILogger, string, Exception?> _loadLobby = LoggerMessage.Define<string>(
        LogLevel.Debug,
        new EventId(BaseCosmosEventId + 2, nameof(LoadLobby)),
        "Loaded lobby {Lobby}");
    private static readonly Action<ILogger, string, Exception?> _saveLobby = LoggerMessage.Define<string>(
        LogLevel.Debug,
        new EventId(BaseCosmosEventId + 3, nameof(SaveLobby)),
        "Saved lobby: {Lobby}");
    private static readonly Action<ILogger, string, Exception?> _deleteLobby = LoggerMessage.Define<string>(
    LogLevel.Debug,
    new EventId(BaseCosmosEventId + 4, nameof(DeleteLobby)),
    "Deleted Lobby: {Lobby}");
    private static readonly Action<ILogger, Exception?> _deleteExpiredLobbies = LoggerMessage.Define(
        LogLevel.Debug,
        new EventId(BaseCosmosEventId + 5, nameof(DeleteExpiredLobbies)),
        "Deleting Expired lobbies.");
    private static readonly Action<ILogger, Exception?> _clearAll = LoggerMessage.Define(
        LogLevel.Debug,
        new EventId(BaseCosmosEventId + 6, nameof(ClearAll)),
        "Deleting all Lobbies.");
    #endregion
    #region Public Methods
    public static void LoadLobbyNames(this ILogger logger) => _loadLobbyNames(logger, null);
    public static void LoadLobby(this ILogger logger, string lobby) => _loadLobby(logger, lobby, null);
    public static void SaveLobby(this ILogger logger, string lobby) => _saveLobby(logger, lobby, null);
    public static void DeleteLobby(this ILogger logger, string lobby) => _deleteLobby(logger, lobby, null);
    public static void DeleteExpiredLobbies(this ILogger logger) => _deleteExpiredLobbies(logger, null);
    public static void ClearAll(this ILogger logger) => _clearAll(logger, null);
    #endregion
    #endregion
    #region Redis Logger
    private const int BaseRedisEventId = 1700;
    #region Private static Actions
    private static readonly Action<ILogger, Exception?> _sendMessage = LoggerMessage.Define(LogLevel.Debug,
        new EventId(BaseRedisEventId + 1, nameof(SendMessage)), "Message sent to Redis.");

    private static readonly Action<ILogger, Exception?> _errorSendMessage = LoggerMessage.Define(LogLevel.Debug,
        new EventId(BaseRedisEventId + 2, nameof(ErrorSendMessage)), "Error sending message to Redis.");

    private static readonly Action<ILogger, string, string, Exception?> _subscriptionCreated = LoggerMessage.Define<string, string>(LogLevel.Debug,
        new EventId(BaseRedisEventId + 3, nameof(SubscriptionCreated)), "Redis subscription was created (Channel: {Channel}, NodeID {NodeId})");

    private static readonly Action<ILogger, string?, string?, string?, Exception?> _messageReceived = LoggerMessage.Define<string?, string?, string?>(LogLevel.Debug,
        new EventId(BaseRedisEventId + 5, nameof(MessageReceived)), "Redis message was received (Channel: {Channel}, NodeID: {NodeId}, MessageID: {MessageId})");

    private static readonly Action<ILogger, string?, string?, string?, Exception?> _messageProcessed = LoggerMessage.Define<string?, string?, string?>(LogLevel.Debug,
        new EventId(BaseRedisEventId + 6, nameof(MessageProcessed)), "Redis message was processed (Channel: {Channel}, NodeID: {NodeId}, MessageID {MessageId}");

    private static readonly Action<ILogger, string?, string?, string?, Exception?> _errorProccessMessage = LoggerMessage.Define<string?, string?, string?>(LogLevel.Debug,
        new EventId(BaseRedisEventId + 7, nameof(ErrorProcessMessage)), "Redis message processing failed (Channel: {Channel}, NodeID: {NodeId}, MessageID: {MessageId})");
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
    #endregion
    #region SignalR Hub Logger
    private const int BaseSignalREventId = 1100;
    #region Private Static Actions
    private static readonly Action<ILogger, string, string, string, Deck, Exception?> _createLobby =
        LoggerMessage.Define<string, string, String, Deck>(
            LogLevel.Information,
            new EventId(BaseSignalREventId + 1, nameof(CreateLobby)),
            "{Action}(\"{Lobby}\", \"{CardTsarName}\", {Deck})");

    private static readonly Action<ILogger, string, string, string, bool, Exception?> _joinLobby =
        LoggerMessage.Define<string, string, String, Boolean>(
            LogLevel.Information,
            new EventId(BaseSignalREventId + 2, nameof(JoinLobby)),
            "{Action}(\"{LobbyName}\", \"{MemberName}\", {AsObserver})");

    private static readonly Action<ILogger, string, string, string, Exception?> _reconnectLobby =
        LoggerMessage.Define<string, string, string>(
            LogLevel.Information,
            new EventId(BaseSignalREventId + 3, nameof(ReconnectLobby)),
            "{Action}(\"{LobbyName}\", \"{MemberName}\")");

    private static readonly Action<ILogger, string, string, string, Exception?> _disconnectLobby =
        LoggerMessage.Define<string, string, string>(
            LogLevel.Information,
            new EventId(BaseSignalREventId + 4, nameof(DisconnectLobby)),
            "{Action}(\"{LobbyName}\", \"{MemberName}\")");

    private static readonly Action<ILogger, string, string, Exception?> _startRound =
        LoggerMessage.Define<string, string>(
            LogLevel.Information,
            new EventId(BaseSignalREventId + 5, nameof(StartRound)),
            "{Action}(\"{LobbyName}\")");

    private static readonly Action<ILogger, string, string, Exception?> _cancelRound =
        LoggerMessage.Define<string, string>(
            LogLevel.Information,
            new EventId(BaseSignalREventId + 6, nameof(CancelRound)),
            "{Action}(\"{LobbyName}\")");

    private static readonly Action<ILogger, string, string, string, Guid?, Exception?> _submitWhiteCard =
        LoggerMessage.Define<string, string, string, Guid?>(
            LogLevel.Information,
            new EventId(BaseSignalREventId + 7, nameof(SubmitWhiteCard)),
            "{Action}(\"{LobbyName}\", \"{MemberName}\", {WhiteCard})");

    private static readonly Action<ILogger, string, string, string, Guid, long, Exception?> _getMessages =
        LoggerMessage.Define<string, string, string, Guid, long>(
            LogLevel.Information,
            new EventId(BaseSignalREventId + 8, nameof(GetMessages)),
            "{Action}(\"{LobbyName}\", \"{MemberName}\", {SessionId}, {LastMessageId})");

    private static readonly Action<ILogger, string, Exception?> _messageReceived = LoggerMessage.Define<string>(
        LogLevel.Debug,
        new EventId(BaseSignalREventId + 9, nameof(MessageReceived)),
        "Notify messages recieved (connectionId: {ConnectionId})");

    private static readonly Action<ILogger, string, string, string, TimeSpan, Exception?> _startTimer =
        LoggerMessage.Define<string, string, String, TimeSpan>(
            LogLevel.Information,
            new EventId(BaseSignalREventId + 10, nameof(StartTimer)),
            "{Action}(\"{LobbyName}\", \"{MessageName}\")");

    private static readonly Action<ILogger, string, string, string, Exception?> _cancelTimer = LoggerMessage.Define<string, string, string>(
        LogLevel.Information,
        new EventId(BaseSignalREventId + 11, nameof(CancelTimer)),
        "{Action}(\"{LobbyName}\", \"{MemberName}\")");

    private static readonly Action<ILogger, string, string, Deck, Exception?> _changeDeck =
        LoggerMessage.Define<string, string, Deck>(
            LogLevel.Information,
            new EventId(BaseSignalREventId + 12, nameof(ChangeDeck)),
            "{Action}(\"{LobbyName}\", {Deck})");
    #endregion
    #region Public Logger Methods
    public static void CreateLobby(this ILogger logger, string lobbyName, string cardTsarName, Deck deck) => _createLobby(logger, nameof(CreateLobby), lobbyName, cardTsarName, deck, null);
    public static void JoinLobby(this ILogger logger, string lobbyName, string memberName, bool asObserver) => _joinLobby(logger, nameof(JoinLobby), lobbyName, memberName, asObserver, null);
    public static void ReconnectLobby(this ILogger logger, string lobbyName, string memberName) => _reconnectLobby(logger, nameof(ReconnectLobby), lobbyName, memberName, null);
    public static void DisconnectLobby(this ILogger logger, string lobbyName, string memberName) => _disconnectLobby(logger, nameof(DisconnectLobby), lobbyName, memberName, null);
    public static void StartRound(this ILogger logger, string lobbyName) => _startRound(logger, nameof(StartRound), lobbyName, null);
    public static void CancelRound(this ILogger logger, string lobbyName) => _cancelRound(logger, nameof(CancelRound), lobbyName, null);
    public static void SubmitWhiteCard(this ILogger logger, string lobbyName, string memberName, Guid? whiteCardId) => _submitWhiteCard(logger, nameof(SubmitWhiteCard), lobbyName, memberName, whiteCardId, null);
    public static void ChangeDeck(this ILogger logger, string lobbyName, Deck deck) => _changeDeck(logger, nameof(ChangeDeck), lobbyName, deck, null);
    public static void StartTimer(this ILogger logger, string lobbyName, string memberName, TimeSpan duration) => _startTimer(logger, nameof(StartTimer), lobbyName, memberName, duration, null);
    public static void CancelTimer(this ILogger logger, string lobbyName, string memberName) => _cancelTimer(logger, nameof(CancelTimer), lobbyName, memberName, null);
    public static void GetMessages(this ILogger logger, string lobbyName, string memberName, Guid sessionId, long lastMessageId) => _getMessages(logger, nameof(GetMessages), lobbyName, memberName, sessionId, lastMessageId, null);
    public static void MessageReceived(this ILogger logger, string connectionId) => _messageReceived(logger, connectionId, null);
    #endregion
    #endregion
}
