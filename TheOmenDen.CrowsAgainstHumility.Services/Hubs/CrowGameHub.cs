using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Engines;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Hubs;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Core.Results;
using TheOmenDen.CrowsAgainstHumility.Services.CrowGameBuilder;

namespace TheOmenDen.CrowsAgainstHumility.Services.Hubs;
public sealed class CrowGameHub : Hub
{
    public const string HubUrl = "/crowGameHub";

    #region Injected Services
    private readonly ICrowGameEngine _crowGameEngine;
    private readonly ICrowGameEventBroadcaster _eventBroadcaster;
    private readonly ILogger<CrowGameHub> _logger;
    #endregion
    #region Constructors
    public CrowGameHub(ICrowGameEngine crowGameEngine, ICrowGameEventBroadcaster eventBroadcaster, ILogger<CrowGameHub> logger)
    {
        _crowGameEngine = crowGameEngine;
        _eventBroadcaster = eventBroadcaster;
        _logger = logger;
    }
    #endregion
    #region Connection Methods
    public async Task Connect(Guid id)
        => await Groups.AddToGroupAsync(GetPlayerPrivateId(), id.ToString());

    public void Clear(Guid serverId) => _crowGameEngine.ClearGameBoard(serverId, GetPlayerPrivateId());

    public async Task ClearAsync(Guid serverId, CancellationToken cancellationToken = default)
        => await _crowGameEngine.ClearGameBoardAsync(serverId, GetPlayerPrivateId(), cancellationToken);

    public ServerCreationResult Create(IEnumerable<Pack> desiredPacks)
    {
        var (wasCreated, serverId, validationMessage) = _crowGameEngine.CreateRoom(desiredPacks);
        var creationResult = new ServerCreationResult
        {
            WasCreated = wasCreated,
            ServerId = serverId,
            ValidationMessage = validationMessage
        };
        return creationResult;
    }

    public async Task<ServerCreationResult> CreateAsync(IEnumerable<Pack> desiredPacks, CancellationToken cancellationToken = default)
    {
        var (wasCreated, serverId, validationMessage) = await _crowGameEngine.CreateRoomAsync(desiredPacks, cancellationToken);
        
        var creationResult = new ServerCreationResult
        {
            WasCreated = wasCreated,
            ServerId = serverId,
            ValidationMessage = validationMessage
        };

        return creationResult;
    }


    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await _crowGameEngine.SleepInAllRoomsAsync(GetPlayerPrivateId());
        await base.OnDisconnectedAsync(exception);
    }
    #endregion
    #region Private Methods
    private string GetPlayerPrivateId() => Context.ConnectionId;
    #endregion
}
