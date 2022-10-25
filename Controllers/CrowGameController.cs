using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;

namespace TheOmenDen.CrowsAgainstHumility.Controllers;
public class CrowGameController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);
}
