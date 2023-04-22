using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Cards;
using TheOmenDen.CrowsAgainstHumility.Core.DTO.ViewModels;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Events;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Models;

namespace TheOmenDen.CrowsAgainstHumility.Core.Engine.Engine;

public interface ICrowGameEngine
{
    event EventHandler<PlayerKickedEventArgs> PlayerKicked;
    event EventHandler<RoomUpdatedEventArgs> RoomUpdated;
    event EventHandler<LogUpdatedEventArgs> LogUpdated;
    event EventHandler<RoomClearedEventArgs> RoomCleared;
    Task KickAsync(Guid serverId, string initiatingPlayerConnectionId, int playerToRemovePublicId, CancellationToken cancellationToken = default);
    Task SetPlayerAsleepInAllRoomsAsync(string playerConnectionId, CancellationToken cancellationToken = default);
    Task PlayCardAsync(Guid serverId, string playerConnectionId, WhiteCard card, CancellationToken cancellationToken = default);
    Task RedactWhiteCardAsync(Guid serverId, string playerConnectionId, CancellationToken cancellationToken = default);
    Task ClearCardsAsync(Guid serverId, string playerConnectionId, CancellationToken cancellationToken = default);
    Task ShowCardsAsync(Guid serverId, string playerConnectionId, CancellationToken cancellationToken = default);
    Task<Player> ChangePlayerTypeAsync(Guid serverId, string playerConnectionId, GameRoles newRole, CancellationToken cancellationToken = default);
    Task<(bool wasCreated, Guid? serverId, string? validationmessages)> CreateServerAsync(CrowGameInputViewModel crowGameBase, CancellationToken cancellationToken = default);
    Task<Player> JoinRoomAsync(Guid serverId, Guid recoveryId, string playerName, string playerConnectionId, GameRoles role, CancellationToken cancellationToken = default);
}