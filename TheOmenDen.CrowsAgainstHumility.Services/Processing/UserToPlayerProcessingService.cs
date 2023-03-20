using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Services.Exceptions;

namespace TheOmenDen.CrowsAgainstHumility.Services.Processing;

internal sealed class UserToPlayerProcessingService : IUserToPlayerProcessingService
{
    private readonly IUserService _userService;
    private readonly ILogger<UserToPlayerProcessingService> _logger;

    public UserToPlayerProcessingService(IUserService userService, ILogger<UserToPlayerProcessingService> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task<Player?> GetPlayerByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var retrievedUser = await _userService.GetUserViewModelAsync(userId, cancellationToken);
        
        return new()
        {
            Username = retrievedUser.Username,
            GameRole = GameRoles.Player,
            Id = retrievedUser.Id
        };
    }

    public async Task<Player?> GetPlayerByUsernameAsync(String username, CancellationToken cancellationToken = default)
    {
        var retrievedUser = await _userService.GetUserByUsernameAsync(username, cancellationToken);
        try
        {
            return new Player
            {
                Username = username,
                GameRole = GameRoles.Player,
                Id = retrievedUser?.Id ?? Guid.Empty
            } ?? throw new UserNotFoundException($"Couldn't find user with username {username}");
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to find user {@Ex}", ex);
            throw;
        }
    }
}
