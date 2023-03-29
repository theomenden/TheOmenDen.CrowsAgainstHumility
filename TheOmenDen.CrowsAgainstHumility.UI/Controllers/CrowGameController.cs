using System.Collections.Concurrent;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;
using TheOmenDen.CrowsAgainstHumility.Core.Providers;

namespace TheOmenDen.CrowsAgainstHumility.Controllers;
public class CrowGameController : Controller
{
    private readonly IDictionary<string, Tuple<PlayerList, object>> _currentPlayers = new ConcurrentDictionary<string, Tuple<PlayerList, object>>(StringComparer.OrdinalIgnoreCase);
    private readonly TaskProvider _taskProvider;
    private ILogger<CrowGameController> _logger;

    public CrowGameController(
        DateTimeProvider? dateTimeProvider,
        GuidProvider? guidProvider,
        TaskProvider? taskProvider,
        ICrowGameConfiguration? configuration,
        IPlayerListService? playerListService,
        ILogger<CrowGameController> logger)
    {
        _deckProvider = deckProvider;
        _taskProvider = taskProvider;
        _logger = logger;
    }
}
