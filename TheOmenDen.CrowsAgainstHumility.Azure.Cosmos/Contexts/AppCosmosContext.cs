using Microsoft.EntityFrameworkCore;
using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;

namespace TheOmenDen.CrowsAgainstHumility.Azure.Cosmos.Contexts;
internal sealed class AppCosmosContext: DbContext
{
    public AppCosmosContext(DbContextOptions<AppCosmosContext> options)
        :base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly()
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<PlayerList> PlayerLists { get; set; }
}
