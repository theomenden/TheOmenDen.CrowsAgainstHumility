using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Settings;
using TheOmenDen.CrowsAgainstHumility.Core.Models.Settings;

namespace TheOmenDen.CrowsAgainstHumility.Services.Settings;
internal class FilePlayerListRepositorySettings: IFilePlayerListRepositorySettings
{
    private readonly ICrowGameConfiguration _configuration;

    public FilePlayerListRepositorySettings(ICrowGameConfiguration? configuration)
    {
        _configuration = configuration ?? new CrowGameConfiguration();
    }

    public string Folder
    {
        get
        {
            var result = _configuration.RepositoryDirectory;

            if (string.IsNullOrWhiteSpace(result))
            {
                result = @"App_Data\Teams";
            }

            result = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, result);
            result = Path.GetFullPath(result);

            return result;
        }
    }
}
