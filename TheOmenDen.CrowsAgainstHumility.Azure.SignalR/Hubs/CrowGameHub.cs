using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Cards;
using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Identity;
using TheOmenDen.CrowsAgainstHumility.Core.DTO.ViewModels;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Engine;
using TheOmenDen.CrowsAgainstHumility.Core.Transformation.Mappers;

namespace TheOmenDen.CrowsAgainstHumility.Azure.SignalR.Hubs;
public class CrowGameHub: Hub
{
    private static readonly Lazy<PlayerMapper> _mapper = new(() => new());
    private readonly ICrowGameEngine _crowGameEngine;
    private readonly ICrowGameHubBroadcaster _eventBroadcaster;
    
    public CrowGameHub(ICrowGameEngine crowGameEngine, ICrowGameHubBroadcaster eventBroadcaster)
    {
        _crowGameEngine = crowGameEngine;
        _eventBroadcaster = eventBroadcaster;
    }

    public Task Connect(Guid serverId, CancellationToken cancellationToken) => Groups.AddToGroupAsync(GetPlayerConnectionId(), serverId.ToString(), cancellationToken);
    public Task KickPlayer(Guid serverId, string initiatingPlayerConnectionId, int playerPublicIdToRemove, CancellationToken cancellationToken = default) => _crowGameEngine.KickAsync(serverId, initiatingPlayerConnectionId, playerPublicIdToRemove, cancellationToken);
    public Task PlayCard(Guid serverId, string playerConnectionId, WhiteCard playedCard, CancellationToken cancellationToken = default) => _crowGameEngine.PlayCardAsync(serverId, playerConnectionId, playedCard, cancellationToken);
    public Task UnPlayCard(Guid serverId, string playerConnectionId, CancellationToken cancellationToken = default) => _crowGameEngine.RedactWhiteCardAsync(serverId, playerConnectionId, cancellationToken);
    public Task ClearBoard(Guid serverId, CancellationToken cancellationToken = default) => _crowGameEngine.ClearCardsAsync(serverId, GetPlayerConnectionId(), cancellationToken);
    public Task ShowCards(Guid serverId, CancellationToken cancellationToken = default) => _crowGameEngine.ShowCardsAsync(serverId, GetPlayerConnectionId(), cancellationToken);
    public async Task<PlayerViewModel> ChangePlayerType(Guid serverId, Core.Engine.Enumerations.GameRoles type, CancellationToken cancellationToken = default)
    {
        var updatedPlayer = await _crowGameEngine.ChangePlayerTypeAsync(serverId, GetPlayerConnectionId(), type, cancellationToken);
        return _mapper.Value.PlayerToPlayerViewModelWithPrivateId(updatedPlayer);
    }
    public async Task<ServerCreationResult> Create(CrowGameInputViewModel crowGameConfiguration, CancellationToken cancellationToken = default)
    {
        var (wasCreated, serverId, validationMessage) = await _crowGameEngine.CreateServerAsync(crowGameConfiguration, cancellationToken);

        return new ServerCreationResult(wasCreated, serverId, validationMessage);
    }
    public async Task<PlayerViewModel> Join(Guid serverId, Guid recoveryId, string playerName, Core.Engine.Enumerations.GameRoles type, CancellationToken cancellationToken = default)
    {
        var joinedPlayer = await _crowGameEngine.JoinRoomAsync(serverId, recoveryId, playerName, GetPlayerConnectionId(), type, cancellationToken);
        return _mapper.Value.PlayerToPlayerViewModelWithPrivateId(joinedPlayer);
    }
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await _crowGameEngine.SetPlayerAsleepInAllRoomsAsync(GetPlayerConnectionId());
        await base.OnDisconnectedAsync(exception);
    }

    private string GetPlayerConnectionId() => Context.ConnectionId;
}
