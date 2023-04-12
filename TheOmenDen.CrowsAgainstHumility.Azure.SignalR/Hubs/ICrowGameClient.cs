namespace TheOmenDen.CrowsAgainstHumility.Azure.SignalR.Hubs;
public interface ICrowGameClient
{
    #region Event Handlers
    event EventHandler<PlayerKickedEventArgs> PlayerKicked;
    event EventHandler<RoomUpdatedEventArgs> RoomUpdated;
    event EventHandler<LogUpdatedEventArgs> LogUpdated;
    event EventHandler<RoomClearedEventArgs> RoomCleared;
    #endregion
    #region Synchronous Methods
    void Kick(Guid id, string initiatingPlayerPrivateId, int playerPublicIdToRemove);
    void SleepInAllRooms(string playerPrivateId);
    (bool wasCreated, Guid? serverid, string? validationMessages) CreateRoom(Deck desiredPlayingDeck);
    void PlayerWhiteCard (Guid serverId, string playerPrivateId, WhiteCard whiteCard);
    void RedactWhiteCard(Guid serverId, string playerPrivateId);
    void ShowWhiteCards(Guid serverId, string playerPrivateId);
    Player ChangePlayerType(Guid serverId, string playerPrivateId, PlayerTypes playerType);
    #endregion
    Task Notify(IEnumerable<Message> messages);
}
