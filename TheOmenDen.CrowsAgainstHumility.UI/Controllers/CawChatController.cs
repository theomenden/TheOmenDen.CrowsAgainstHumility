using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TheOmenDen.CrowsAgainstHumility.Identity.Contexts;

namespace TheOmenDen.CrowsAgainstHumility.Controllers;

[Authorize]
public class CawChatController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IDbContextFactory<AuthDbContext> _dbContextFactory;

    public CawChatController(UserManager<ApplicationUser> userManager, IDbContextFactory<AuthDbContext> dbContextFactory)
    {
        _userManager = userManager;
        _dbContextFactory = dbContextFactory;
    }

    [Authorize]
    [HttpGet("{contactId:guid}")]
    public async Task<IActionResult> GetConversationAsync(Guid contactId, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var userId = User.Claims.Where(a => a.Type == ClaimTypes.NameIdentifier).Select(a => a.Value).FirstOrDefault();

        if (!Guid.TryParse(userId, out var userGuid) && userGuid != Guid.Empty)
        {
            return BadRequest();
        }

        var messages = await context.ChatMessages
            .Where(m => (m.FromUserId == contactId && m.ToUserId == userGuid)
                        || (m.FromUserId == userGuid && m.ToUserId == contactId))
            .OrderBy(a => a.CreatedAt)
            .Include(a => a.FromUser)
            .Include(a => a.ToUser)
            .Select(m => new CawChatMessageDto(m.Id, m.CreatedAt, m.ToUser, m.FromUser, m.Message))
            .ToListAsync(cancellationToken);

        return Ok(messages);
    }

    [Authorize]
    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetUserDetailsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> SaveMessageAsync(CawChatMessageDto messageDto, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var chatMessage = new CawChatMessage
        {
            Message = messageDto.Message,
            FromUser = messageDto.FromUser,
            FromUserId = messageDto.FromUser.Id,
            ToUser = messageDto.ToUser,
            ToUserId = messageDto.ToUser.Id,
            CreatedAt = messageDto.CreatedAt
        };

        await context.ChatMessages.AddAsync(chatMessage, cancellationToken);

        return Ok(await context.SaveChangesAsync(cancellationToken));
    }
}
