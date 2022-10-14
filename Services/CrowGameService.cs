using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using TheOmenDen.CrowsAgainstHumility.Core.Delegates;
using TheOmenDen.CrowsAgainstHumility.Core.Messages;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Core.Models.EventArgs;
using TheOmenDen.CrowsAgainstHumility.Hubs;
using TheOmenDen.Shared.Guards;

namespace TheOmenDen.CrowsAgainstHumility.Services;

public sealed class CrowGameService
{
    public Player Player { get; set; } = new();

    public CrowGame Game { get; set; } = new();

    public bool IsStateReady => !(String.IsNullOrWhiteSpace(Player.Username) || String.IsNullOrWhiteSpace(Game.Name));
}