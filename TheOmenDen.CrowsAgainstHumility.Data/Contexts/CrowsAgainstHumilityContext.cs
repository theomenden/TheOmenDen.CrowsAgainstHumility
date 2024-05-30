using Microsoft.EntityFrameworkCore;
using TheOmenDen.CrowsAgainstHumility.Core.Models.Cards;

namespace TheOmenDen.CrowsAgainstHumility.Data.Contexts;

public sealed class CrowsAgainstHumilityContext(DbContextOptions<CrowsAgainstHumilityContext> options) : DbContext(options)
{
    public DbSet<BlackCard> BlackCards { get; set; }
    public DbSet<WhiteCard> WhiteCards { get; set; }
    public DbSet<Expansion> Expansions { get; set; }

}