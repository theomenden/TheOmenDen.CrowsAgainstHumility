using Azure.Identity;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace TheOmenDen.CrowsAgainstHumility.Identity.Contexts;
internal sealed class DesignAuthDbContext : IDesignTimeDbContextFactory<AuthDbContext>
{
    private IConfigurationRoot _configuration;

    public AuthDbContext CreateDbContext(string[] args)
    {
        var vaultUri = Environment.GetEnvironmentVariable("VaultUri") ?? String.Empty;

        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true, true);

        if (!String.IsNullOrEmpty(vaultUri))
        {
            builder.AddAzureKeyVault(new Uri(vaultUri), new DefaultAzureCredential());
        }

        _configuration = builder.Build();

        var crowsAgainstAuth = _configuration["ConnectionStrings:CrowsAgainstAuthority"]
                                                      ?? _configuration["ConnectionStrings:CrowsAgainstHumilityDb"];

        var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();
        optionsBuilder.UseSqlServer(crowsAgainstAuth);
        
        return new AuthDbContext(optionsBuilder.Options);
    }
}
