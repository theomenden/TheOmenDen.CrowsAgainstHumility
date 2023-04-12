using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Engines;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Hubs;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;

namespace TheOmenDen.CrowsAgainstHumility.Azure.SignalR.Hubs;
public class CrowGameHub: Hub
{
    private readonly ICrowGameEngine _crowGameEngine;
    private readonly ICrowGameEventBroadcaster _eventBroadcaster;
    private readonly ILogger<CrowGameHub> _logger;

    public CrowGameHub(ICrowGameEngine crowGameEngine, ICrowGameEventBroadcaster eventBroadcaster, ILogger<CrowGameHub> logger)
    {
        _crowGameEngine = crowGameEngine;
        _eventBroadcaster = eventBroadcaster;
        _logger = logger;
    }

    public async Task Connect(Guid id, CancellationToken cancellationToken = default)
        => await Groups.AddToGroupAsync(GetPlayerPrivateId(), id.ToString(), cancellationToken: cancellationToken);

    public void Kick(Guid id, string initiatingPlayerPrivateId, int playerPublicIdToRemove)
        => _crowGameEngine.Kick(id, initiatingPlayerPrivateId, playerPublicIdToRemove);

    public Member Join(Guid id, Guid recoveryId, string playerName, GameRoles type)
    {
        var joinedPlayer = _crowGameEngine.JoinRoom(id, recoveryId, playerName, GetPlayerPrivateId(), type);
        return joinedPlayer.Map(true);
    }

    public void PlayWhiteCard(Guid serverId, string playerId, WhiteCard playedCard) =>
        _crowGameEngine.PlayWhiteCard(serverId, playerId, playedCard);

    public void UnPlayWhiteCard(Guid serverId, string playerId)
    => _crowGameEngine.RedactPlayedWhiteCard(serverId, playerId);

    public void Clear(Guid serverId) => _crowGameEngine.ClearGameBoard(serverId, GetPlayerPrivateId());

    public void Show(Guid serverId) => _crowGameEngine.ShowWhiteCards(serverId, GetPlayerPrivateId());

    public Member ChangePlayerType(Guid serverId, GameRoles newType)
    {
        var updatedPlayer = _crowGameEngine.ChangePlayerType(serverId, GetPlayerPrivateId(), newType);
        return updatedPlayer.Map(includePrivateId: true);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await _crowGameEngine.SleepInAllRoomsAsync(GetPlayerPrivateId());
        await base.OnDisconnectedAsync(exception);
    }

    private string GetPlayerPrivateId() => Context.ConnectionId;
}
