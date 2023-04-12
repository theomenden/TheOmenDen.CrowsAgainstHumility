using Microsoft.Extensions.Logging;

namespace TheOmenDen.CrowsAgainstHumility.Services.Extensions;
internal static class FilePlayerListRepositoryLogger
{
    #region Constants
    private const int BaseEventId = 1050;
    #endregion
    #region Private static Actions
    private static readonly Action<ILogger, Exception?> _loadPlayerListNames = LoggerMessage.Define(
        LogLevel.Debug,
        new EventId(BaseEventId + 1, nameof(LoadPlayerListNames)),
            "Loading Player List names");
    private static readonly Action<ILogger, string, Exception?> _loadPlayerList = LoggerMessage.Define<string>(
        LogLevel.Debug,
        new EventId(BaseEventId + 2, nameof(LoadPlayerList)),
        "Loading Player List: {PlayerList}");
    private static readonly Action<ILogger, string, Exception?> _savePlayerList = LoggerMessage.Define<string>(
        LogLevel.Debug,
        new EventId(BaseEventId + 3, nameof(SavePlayerList)),
        "Saved Player List: {PlayerList}");
    private static readonly Action<ILogger, string, Exception?> _deletePlayerList = LoggerMessage.Define<string>(
        LogLevel.Debug,
        new EventId(BaseEventId + 4, nameof(DeletePlayerList)),
        "Deleted Player list: {PLayerList}");
    private static readonly Action<ILogger, Exception?> _deleteExpiredPlayerLists = LoggerMessage.Define(
        LogLevel.Debug,
        new EventId(BaseEventId + 5, nameof(DeleteExpiredPlayerLists)),
        "Deleting expired Player Lists");
    private static readonly Action<ILogger, Exception?> _deleteAll = LoggerMessage.Define(
        LogLevel.Debug,
        new EventId(BaseEventId + 6, nameof(DeleteAll)),
        "Deleting all Player Lists");
    #endregion
    #region Public Logger Extensions
    public static void LoadPlayerListNames(this ILogger logger) => _loadPlayerListNames(logger, null);
    public static void LoadPlayerList(this ILogger logger, string playerList) => _loadPlayerList(logger, playerList, null);
    public static void SavePlayerList(this ILogger logger, string playerList) => _savePlayerList(logger, playerList, null);
    public static void DeletePlayerList(this ILogger logger, string playerList) => _deletePlayerList(logger, playerList, null);
    public static void DeleteExpiredPlayerLists(this ILogger logger) => _deleteExpiredPlayerLists(logger, null);
    public static void DeleteAll(this ILogger logger) => _deleteAll(logger, null);
    #endregion
}
