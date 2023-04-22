using Microsoft.Extensions.Logging;

namespace TheOmenDen.CrowsAgainstHumility.Azure.CosmosDb.Extensions;
internal static class LoggingExtensions
{
    private const int BaseEventId = 1050;
    #region Private Actions
    private static readonly Action<ILogger, Exception?> _loadLobbyNames = LoggerMessage.Define(
        LogLevel.Debug,
        new EventId(BaseEventId + 1, nameof(LoadLobbyNames)),
        "Loading Lobby Names.");
    private static readonly Action<ILogger, string, Exception?> _loadLobby = LoggerMessage.Define<string>(
        LogLevel.Debug,
        new EventId(BaseEventId + 2, nameof(LoadLobby)),
        "Loaded lobby {Lobby}");
    private static readonly Action<ILogger, string, Exception?> _saveLobby = LoggerMessage.Define<string>(
        LogLevel.Debug,
        new EventId(BaseEventId + 3, nameof(SaveLobby)),
        "Saved lobby: {Lobby}");
    private static readonly Action<ILogger, string, Exception?> _deleteLobby = LoggerMessage.Define<string>(
    LogLevel.Debug,
    new EventId(BaseEventId + 4, nameof(DeleteLobby)),
    "Deleted Lobby: {Lobby}");
    private static readonly Action<ILogger, Exception?> _deleteExpiredLobbies = LoggerMessage.Define(
        LogLevel.Debug,
        new EventId(BaseEventId + 5, nameof(DeleteExpiredLobbies)),
        "Deleting Expired lobbies.");
    private static readonly Action<ILogger, Exception?> _clearAll = LoggerMessage.Define(
        LogLevel.Debug,
        new EventId(BaseEventId + 6, nameof(ClearAll)),
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
}
