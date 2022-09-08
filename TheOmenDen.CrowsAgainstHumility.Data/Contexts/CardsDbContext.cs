using TheOmenDen.CrowsAgainstHumility.Data.Contexts.Configurations;

namespace TheOmenDen.CrowsAgainstHumility.Data.Contexts;
public class CardsDbContext : DbContext
{
    public CardsDbContext(DbContextOptions<CardsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<PackData> Packs { get; set; } = null!;
           
    public virtual DbSet<BlackCard> BlackCards { get; set; } = null!;

    public virtual DbSet<WhiteCard> WhiteCards { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CardPackConfiguration());
        modelBuilder.ApplyConfiguration(new BlackCardConfiguration());
        modelBuilder.ApplyConfiguration(new WhiteCardConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}