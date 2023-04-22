using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Identity;

#nullable disable
namespace TheOmenDen.CrowsAgainstHumility.Identity.Contexts;
public sealed class AuthDbContext: IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        :base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        var assemblyContext = typeof(Configurations.ApplicationRoleConfiguration).Assembly;

        builder.ApplyConfigurationsFromAssembly(assemblyContext);
        base.OnModelCreating(builder);
    }

    public DbSet<EmailConfiguration> EmailConfigurations { get; set; }

    public DbSet<CawChatMessage> ChatMessages { get; set; }
}
