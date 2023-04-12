using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Services.Extensions;
internal static class CrowGameControllerLogger
{
    #region Constants
    private const int BaseEventId = 1000;
    #endregion
    #region Private Static Actions
    private static readonly Action<ILogger, string, string, Exception?> _lobbyCreated =
        LoggerMessage.Define<string, string>(
            LogLevel.Information,
            new EventId(BaseEventId + 1, nameof(LobbyCreated)),
            "Lobby \"{LobbyName}\" was created with Initial Card Tsar: {InitialCardTsar}");

    private static readonly Action<ILogger, string, Exception?> _lobbyAttached = LoggerMessage.Define<string>(
        LogLevel.Information,
        new EventId(BaseEventId + 2, nameof(LobbyAttached)),
        "Lobby \"{LobbyName}\" was attached.");

    private static readonly Action<ILogger, string, Exception?> _readLobby = LoggerMessage.Define<string>(
        LogLevel.Debug,
        new EventId(BaseEventId + 3, nameof(_readLobby)),
        "Lobby \"{LobbyName}\" was locked.");

    private static readonly Action<ILogger, string, string, bool, Exception?> _observerMessageReceived =
        LoggerMessage.Define<string, string, bool>(
            LogLevel.Debug,
            new EventId(BaseEventId + 4, nameof(ObserverMessageReceived)),
            "Observer \"{Observer}\" in Lobby \"{LobbyName}\" received message: {HasMessages}");

    private static readonly Action<ILogger, string, Exception?> _disconnectingInactiveObservers =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(BaseEventId + 5, nameof(DisconnectingInactiveObservers)),
            "Disconnecting inactive observers in Lobby: {LobbyName}");

    private static readonly Action<ILogger, string, Exception?> _debugLobbyAdded = LoggerMessage.Define<string>(
        LogLevel.Debug,
        new EventId(BaseEventId + 6, nameof(DebugLobbyAdded)),
        "Lobby \"{LobbyName}\" was added.");

    private static readonly Action<ILogger, string, Exception?> _debugLobbyRemoved = LoggerMessage.Define<string>(
        LogLevel.Debug,
        new EventId(BaseEventId + 7, nameof(DebugLobbyRemoved)),
        "Lobby \"{LobbyName}\" was removed.");

    private static readonly Action<ILogger, string, Exception?> _lobbyRemoved = LoggerMessage.Define<string>(
        LogLevel.Debug,
        new EventId(BaseEventId + 8, nameof(LobbyRemoved)),
        "Lobby \"{LobbyName}\" was removed.");
    
    private static readonly Action<ILogger, string, long, MessageTypes, Exception?> _lobbyMessaged =
        LoggerMessage.Define<string, long, MessageTypes>(
            LogLevel.Information,
            new EventId(BaseEventId + 9, nameof(LobbyMessaged)),
            "Lobby Message (lobby: {LobbyName}, ID: {MessageId}, type: {MessageType}, member: {Observer})");

    private static readonly Action<ILogger, string, long, MessageTypes, string?, Exception?> _memberMessaged = LoggerMessage.Define<string, long, MessageTypes, string?>(
        LogLevel.Debug,
        new EventId(BaseEventId + 10, nameof(MemberMessaged)),
        "Lobby Member \"{LobbyName}\" (lobby: {LobbyName}, ID: {MessageId}, type: {MessageType}, member: {Observer})");
    #endregion
    #region Logging Methods
    public static void LobbyCreated(this ILogger logger, string lobbyName, string initialCardTsar) => _lobbyCreated(logger,  lobbyName, initialCardTsar, null);
    public static void LobbyAttached(this ILogger logger, string lobbyName) => _lobbyAttached(logger, lobbyName, null);
    public static void ReadLobby(this ILogger logger, string lobbyName) => _readLobby(logger, lobbyName, null);
    public static void ObserverMessageReceived(this ILogger logger, string lobbyName, string observer, bool hasMessages) => _observerMessageReceived(logger, lobbyName, observer, hasMessages, null);
    public static void DisconnectingInactiveObservers(this ILogger logger, string lobbyName) => _disconnectingInactiveObservers(logger, lobbyName, null);
    public static void DebugLobbyAdded(this ILogger logger, string lobbyName) => _debugLobbyAdded(logger, lobbyName, null);
    public static void DebugLobbyRemoved(this ILogger logger, string lobbyName) => _debugLobbyRemoved(logger, lobbyName, null);
    public static void LobbyRemoved(this ILogger logger, string lobbyName) => _lobbyRemoved(logger, lobbyName, null);
    public static void LobbyMessaged(this ILogger logger, string lobbyName, long messageId, MessageTypes messageType) => _lobbyMessaged(logger, lobbyName, messageId, messageType, null);
    public static void MemberMessaged(this ILogger logger, string lobbyName, long messageId, MessageTypes messageType,
        string? observer) => _memberMessaged(logger, lobbyName, messageId, messageType, observer, null);
    #endregion
}
