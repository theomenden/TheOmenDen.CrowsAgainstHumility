using System.Data.Common;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;
using TheOmenDen.CrowsAgainstHumility.Core.Models.EventArgs;

namespace TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Engines;
public interface ICrowGameEngine
{
    #region EventHandlers
    event EventHandler<PlayerKickedEventArgs> PlayerKicked;
    event EventHandler<RoomUpdatedEventArgs> RoomUpdated;
    event EventHandler<LogUpdatedEventArgs> LogUpdated;
    event EventHandler<RoomClearedEventArgs> RoomCleared;
    #endregion
    #region Synchronous Methods
    void Kick(Guid id, string initiatingPlayerPrivateId, int playerPublicIdToRemove);
    void SleepInAllRooms(string playerPrivateId);
    (bool wasCreated, Guid? serverId, string? validationMessages) CreateRoom(Deck deck);
    Observer JoinRoom(Guid id, Guid recoveryId, string playerName, string playerPrivateId, GameRoles playerType);
    void PlayWhiteCard(Guid serverId, string playerPrivateId, WhiteCard playedCard);
    void RedactPlayedWhiteCard(Guid serverId, string playerPrivateId);
    void ClearGameBoard(Guid serverId, string playerPrivateId);
    void ShowWhiteCards(Guid serverId, string playerPrivateId);
    Member ChangePlayerType(Guid serverId, string playerPrivateId, GameRoles newPlayerType);
    #endregion
    #region Asynchronous Methods
    Task KickAsync(Guid id, string initiatingPlayerPrivateId, int playerPublicIdToRemove, CancellationToken cancellationToken = default);
    Task SleepInAllRoomsAsync(string playerPrivateId, CancellationToken cancellationToken = default);
    Task<(bool wasCreated, Guid? serverId, string? validationMessages)> CreateRoomAsync(IEnumerable<Pack> packs, CancellationToken cancellationToken = default);
    Task<Observer> JoinRoomAsync(Guid id, Guid recoveryId, string playerName, string playerPrivateId, GameRoles playerType, CancellationToken cancellationToken = default);
    Task PlayWhiteCardAsync(Guid serverId, string playerPrivateId, CancellationToken cancellationToken = default);
    Task ClearGameBoardAsync(Guid serverId, string playerPrivateId, CancellationToken cancellationToken = default);
    Task ShowWhiteCardsAsync(Guid serverId, string playerPrivateId, CancellationToken cancellationToken = default);
    Task<Member> ChangePlayerTypeAsync(Guid serverId, string playerPrivateId, GameRoles newPlayerType, CancellationToken cancellationToken = default);
    #endregion
}
