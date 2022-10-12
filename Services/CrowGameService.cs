using Microsoft.AspNetCore.SignalR;
using TheOmenDen.CrowsAgainstHumility.Hubs;

namespace TheOmenDen.CrowsAgainstHumility.Services
{
    public sealed class CrowGameService
    {
        private readonly ILogger<CrowGameService> _logger;
        private readonly IHubContext<CrowGameHub, ICrowGame> _hubContext;

        public CrowGameService(ILogger<CrowGameService> logger, IHubContext<CrowGameHub, ICrowGame> hubContext)
        {
            _logger = logger;

            _hubContext = hubContext;
        }
    }
}
