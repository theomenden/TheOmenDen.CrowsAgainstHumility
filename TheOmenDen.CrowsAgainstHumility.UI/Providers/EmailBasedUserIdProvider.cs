using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TheOmenDen.CrowsAgainstHumility.Providers;

public sealed class EmailBasedUserIdProvider: IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection) => connection.User?.FindFirst(ClaimTypes.Email)?.Value ?? String.Empty;
}
