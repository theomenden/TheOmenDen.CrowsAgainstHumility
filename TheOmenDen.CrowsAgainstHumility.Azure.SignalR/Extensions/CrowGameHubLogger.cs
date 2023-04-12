using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;

namespace TheOmenDen.CrowsAgainstHumility.Azure.SignalR.Extensions;
internal static class CrowGameHubLogger
{
    #region Constants
    private const int BaseEventId = 1100;
    #endregion
    #region Private Static Actions
    private static readonly Action<ILogger, string, string, string, Deck, Exception?> _createLobby =
        LoggerMessage.Define<string, string, String, Deck>(
            LogLevel.Information,
            new EventId(BaseEventId + 1, nameof(CreateLobby)),
            "{Action}(\"{Lobby}\", \"{CardTsarName}\", {Deck})");

    private static readonly Action<ILogger, string, string, string, bool, Exception?> _joinLobby =
        LoggerMessage.Define<string, string, String, Boolean>(
            LogLevel.Information,
            new EventId(BaseEventId + 2, nameof(JoinLobby)),
            "{Action}(\"{LobbyName}\", \"{MemberName}\", {AsObserver})");

    private static readonly Action<ILogger, string, string, string, Exception?> _reconnectLobby =
        LoggerMessage.Define<string, string, string>(
            LogLevel.Information,
            new EventId(BaseEventId + 3, nameof(ReconnectLobby)),
            "{Action}(\"{LobbyName}\", \"{MemberName}\")");

    private static readonly Action<ILogger, string, string, string, Exception?> _disconnectLobby =
        LoggerMessage.Define<string, string, string>(
            LogLevel.Information,
            new EventId(BaseEventId + 4, nameof(DisconnectLobby)),
            "{Action}(\"{LobbyName}\", \"{MemberName}\")");

    private static readonly Action<ILogger, string, string, Exception?> _startRound =
        LoggerMessage.Define<string, string>(
            LogLevel.Information,
            new EventId(BaseEventId + 5, nameof(StartRound)),
            "{Action}(\"{LobbyName}\")");

    private static readonly Action<ILogger, string, string, Exception?> _cancelRound =
        LoggerMessage.Define<string, string>(
            LogLevel.Information,
            new EventId(BaseEventId + 6, nameof(CancelRound)),
            "{Action}(\"{LobbyName}\")");

    private static readonly Action<ILogger, string, string, string, Guid?, Exception?> _submitWhiteCard =
        LoggerMessage.Define<string, string, string, Guid?>(
            LogLevel.Information,
            new EventId(BaseEventId + 7, nameof(SubmitWhiteCard)),
            "{Action}(\"{LobbyName}\", \"{MemberName}\", {WhiteCard})");

    private static readonly Action<ILogger, string, string, string, Guid, long, Exception?> _getMessages =
        LoggerMessage.Define<string, string, string, Guid, long>(
            LogLevel.Information,
            new EventId(BaseEventId + 8, nameof(GetMessages)),
            "{Action}(\"{LobbyName}\", \"{MemberName}\", {SessionId}, {LastMessageId})");

    private static readonly Action<ILogger, string, Exception?> _messageReceived = LoggerMessage.Define<string>(
        LogLevel.Debug,
        new EventId(BaseEventId + 9, nameof(MessageReceived)),
        "Notify messages recieved (connectionId: {ConnectionId})");

    private static readonly Action<ILogger, string, string, string, TimeSpan, Exception?> _startTimer =
        LoggerMessage.Define<string, string, String, TimeSpan>(
            LogLevel.Information,
            new EventId(BaseEventId + 10, nameof(StartTimer)),
            "{Action}(\"{LobbyName}\", \"{MessageName}\")");

    private static readonly Action<ILogger, string, string, string, Exception?> _cancelTimer = LoggerMessage.Define<string, string, string>(
        LogLevel.Information,
        new EventId(BaseEventId + 11, nameof(CancelTimer)),
        "{Action}(\"{LobbyName}\", \"{MemberName}\")");

    private static readonly Action<ILogger, string, string, Deck, Exception?> _changeDeck =
        LoggerMessage.Define<string, string, Deck>(
            LogLevel.Information,
            new EventId(BaseEventId + 12, nameof(ChangeDeck)),
            "{Action}(\"{LobbyName}\", {Deck})");
    #endregion
    #region Public Logger Methods
    public static void CreateLobby(this ILogger logger, string lobbyName, string cardTsarName, Deck deck) => _createLobby(logger, nameof(CreateLobby), lobbyName, cardTsarName, deck, null);
    public static void JoinLobby(this ILogger logger, string lobbyName, string memberName, bool asObserver) => _joinLobby(logger, nameof(JoinLobby), lobbyName, memberName, asObserver, null);
    public static void ReconnectLobby(this ILogger logger, string lobbyName, string memberName) => _reconnectLobby(logger, nameof(ReconnectLobby), lobbyName, memberName, null);
    public static void DisconnectLobby(this ILogger logger, string lobbyName, string memberName) => _disconnectLobby(logger, nameof(DisconnectLobby),  lobbyName, memberName, null);
    public static void StartRound(this ILogger logger, string lobbyName) => _startRound(logger, nameof(StartRound), lobbyName, null);
    public static void CancelRound(this ILogger logger, string lobbyName) => _cancelRound(logger, nameof(CancelRound), lobbyName, null);
    public static void SubmitWhiteCard(this ILogger logger, string lobbyName, string memberName, Guid? whiteCardId) => _submitWhiteCard(logger, nameof(SubmitWhiteCard), lobbyName, memberName, whiteCardId, null);
    public static void ChangeDeck(this ILogger logger, string lobbyName, Deck deck) => _changeDeck(logger, nameof(ChangeDeck), lobbyName, deck, null);
    public static void StartTimer(this ILogger logger, string lobbyName, string memberName, TimeSpan duration) => _startTimer(logger, nameof(StartTimer), lobbyName, memberName,  duration, null);
    public static void CancelTimer(this ILogger logger, string lobbyName, string memberName) => _cancelTimer(logger, nameof(CancelTimer), lobbyName, memberName, null);
    public static void GetMessages(this ILogger logger, string lobbyName, string memberName, Guid sessionId, long lastMessageId) => _getMessages(logger, nameof(GetMessages), lobbyName, memberName, sessionId, lastMessageId, null);
    public static void MessageReceived(this ILogger logger, string connectionId) => _messageReceived(logger, connectionId, null);
    #endregion
}
