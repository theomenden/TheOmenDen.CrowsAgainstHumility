using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace TheOmenDen.CrowsAgainstHumility.Data.Contexts;
internal sealed class CrowsAgainstHumilityContextFactory : IDesignTimeDbContextFactory<CrowsAgainstHumilityContext>
{
    private IConfigurationRoot _configuration;

    public CrowsAgainstHumilityContext CreateDbContext(string[] args)
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

        var appDbConnection = _configuration["ConnectionStrings:UserContextConnection"]
                              ?? _configuration["ConnectionStrings:CrowsAgainstHumilityDb"];


        var optionsBuilder = new DbContextOptionsBuilder<CrowsAgainstHumilityContext>();
        optionsBuilder.UseSqlServer(appDbConnection);

        return new CrowsAgainstHumilityContext(optionsBuilder.Options);
    }
}
