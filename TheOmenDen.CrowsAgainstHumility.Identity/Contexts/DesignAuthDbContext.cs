using Microsoft.EntityFrameworkCore.Design;

namespace TheOmenDen.CrowsAgainstHumility.Identity.Contexts;
internal sealed class DesignAuthDbContext : IDesignTimeDbContextFactory<AuthDbContext>
{
    public AuthDbContext CreateDbContext(string[] args)
    {
        
        var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();
        optionsBuilder.UseSqlServer();
        
        return new AuthDbContext(optionsBuilder.Options);
    }
}
