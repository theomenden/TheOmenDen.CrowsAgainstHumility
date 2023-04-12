using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheOmenDen.CrowsAgainstHumility.Core.Models.Settings;

namespace TheOmenDen.CrowsAgainstHumility.Controllers;

public class ConfigurationController: Controller
{
    public ConfigurationController(CrowGameConfiguration clientConfiguration)
    {
        ClientConfiguration = clientConfiguration ?? throw new ArgumentNullException(nameof(clientConfiguration));
    }

    public CrowGameConfiguration ClientConfiguration { get; }

    [Authorize]
    [HttpGet]
    public IActionResult GetConfiguration() => Ok(ClientConfiguration);
}
