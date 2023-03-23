using Microsoft.EntityFrameworkCore.Design;

namespace TheOmenDen.CrowsAgainstHumility.Data.Contexts;
internal sealed class CrowsAgainstHumilityContextFactory : IDesignTimeDbContextFactory<CrowsAgainstHumilityContext>
{
    public CrowsAgainstHumilityContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CrowsAgainstHumilityContext>();
        optionsBuilder.UseSqlServer();

        return new CrowsAgainstHumilityContext(optionsBuilder.Options);
    }
}
