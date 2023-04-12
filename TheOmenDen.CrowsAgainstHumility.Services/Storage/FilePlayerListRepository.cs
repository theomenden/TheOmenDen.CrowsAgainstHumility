using System.Globalization;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Settings;
using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;
using TheOmenDen.CrowsAgainstHumility.Core.Providers;
using TheOmenDen.CrowsAgainstHumility.Core.Serialization;
using TheOmenDen.CrowsAgainstHumility.Services.Extensions;
using TheOmenDen.Shared.Extensions;
using TheOmenDen.Shared.Guards;

namespace TheOmenDen.CrowsAgainstHumility.Services.Storage;
internal class FilePlayerListRepository : IDisposable
{
    #region Constants
    private const char SpecialCharacter = '%';
    private const string FileExtension = ".json";
    #endregion
    #region Private Members
    private readonly IFilePlayerListRepositorySettings _settings;
    private readonly ICrowGameConfiguration _configuration;
    private readonly PlayerListSerializer _playerListSerializer;
    private readonly DateTimeProvider _dateTimeProvider;
    private readonly Lazy<string> _folder;
    private readonly Lazy<char[]> _invalidCharacters;
    private readonly ILogger<FilePlayerListRepository> _logger;
    #endregion
    #region Constructors
    public FilePlayerListRepository(IFilePlayerListRepositorySettings settings, ICrowGameConfiguration configuration, PlayerListSerializer playerListSerializer, DateTimeProvider dateTimeProvider, Lazy<string> folder, Lazy<char[]> invalidCharacters, ILogger<FilePlayerListRepository> logger)
    {
        _settings = settings;
        _configuration = configuration;
        _playerListSerializer = playerListSerializer;
        _dateTimeProvider = dateTimeProvider;
        _folder = folder;
        _invalidCharacters = invalidCharacters;
        _logger = logger;
        StringBuilderPoolFactory<FilePlayerListRepository>.Create(nameof(FilePlayerListRepository));
    }
    #endregion
    #region Public Properties
    public string Folder => _folder.Value;

    public IEnumerable<string> PlayerListNames
    {
        get
        {
            _logger.LoadPlayerListNames();

            var expirationTime = _dateTimeProvider.UtcNow - _configuration.RepositoryPlayerListExpiration;
            var directory = new DirectoryInfo(Folder);

            if (directory.Exists)
            {
                var files = directory.GetFiles("*" + FileExtension);
                foreach (var file in files)
                {
                    if (file.LastWriteTimeUtc < expirationTime)
                    {
                        continue;
                    }

                    var playerListName = GetPlayerListName(file.Name);

                    if (playerListName is not null)
                    {
                        yield return playerListName;
                    }
                }
            }
        }
    }

    #endregion
    #region Public Methods
    public PlayerList? LoadPlayerList(string playerListName)
    {
        Guard.FromNullOrWhitespace(playerListName, nameof(playerListName));

        var file = GetFileName(playerListName);

        file = Path.Combine(Folder, file);

        PlayerList? result = null;

        if (File.Exists(file))
        {
            try
            {
                using var stream = File.OpenRead(file);

                result = _playerListSerializer.Deserialize(stream);
            }
            catch (IOException io)
            {
                _logger.LogError("File is not accessible: {@Ex}", io);
                result = null;
            }
            catch (JsonException ex)
            {
                _logger.LogError("Corrupted file found, unable to parse: {@Ex}", ex);
                result = null;
            }
        }

        if (result is not null)
        {
            _logger.LoadPlayerList(result.Name);
        }

        return result;
    }

    public void SavePlayerList(PlayerList playerList)
    {
        if (playerList is null)
        {
            throw new ArgumentNullException(nameof(playerList));
        }

        InitializeFolder();

        var file = GetFileName(playerList.Name);

        file = Path.Combine(Folder, file);

        using var stream = File.Create(file);

        _playerListSerializer.Serialize(stream, playerList);

        _logger.SavePlayerList(playerList.Name);
    }

    public void DeletePlayerList(string playerListName)
    {
        Guard.FromNullOrWhitespace(playerListName, nameof(playerListName));

        var file = GetFileName(playerListName);

        file = Path.Combine(Folder, file);

        if (!File.Exists(file))
        {
            return;
        }

        try
        {
            File.Delete(file);
            _logger.DeletePlayerList(playerListName);
        }
        catch (IOException io)
        {
            _logger.LogInformation("File may have been deleted: {@Ex}", io);
        }
    }

    public void DeleteExpiredPlayerLists()
    {
        _logger.DeleteExpiredPlayerLists();

        var expirationTime = _dateTimeProvider.UtcNow - _configuration.RepositoryPlayerListExpiration;
        var directory = new DirectoryInfo(Folder);

        if (!directory.Exists)
        {
            return;
        }

        var files = directory.GetFiles("*" + FileExtension);
        foreach (var file in files)
        {
            try
            {
                if (file.LastWriteTimeUtc < expirationTime)
                {
                    file.Delete();
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("File may have been deleted: {@Ex}", ex);
            }
        }
    }

    public void DeleteAll()
    {
        _logger.DeleteAll();

        var directory = new DirectoryInfo(Folder);

        if (!directory.Exists)
        {
            return;
        }

        var files = directory.GetFiles("*" + FileExtension);
        foreach (var fileInfo in files)
        {
            try
            {
                fileInfo.Delete();
            }
            catch (Exception ex)
            {
                _logger.LogInformation("File may have been deleted: {@Ex}", ex);
            }
        }
    }
    #endregion

    #region Private Static Messages

    private static char[] GetInvalidCharacters()
    {
        var invalidFileCharacters = Path.GetInvalidFileNameChars();
        var result = new char[invalidFileCharacters.Length + 1];
        result[0] = SpecialCharacter;
        invalidFileCharacters.CopyTo(result, 1);
        Array.Sort(result, Comparer<char>.Default);
        return result;
    }

    private static string? GetPlayerListName(string fileName)
    {
        var name = Path.GetFileNameWithoutExtension(fileName);

        var result = StringBuilderPoolFactory<FilePlayerListRepository>.Get(nameof(FilePlayerListRepository)) ??
                     new StringBuilder(name.Length);

        var specialPosition = 0;

        for (var i = 0; i < name.Length; i++)
        {
            var c = name[i];

            if (specialPosition is not 0)
            {
                specialPosition--;
                continue;
            }

            if (c is not SpecialCharacter)
            {
                result.Append(c);
                continue;
            }

            if (name.Length <= i + 4)
            {
                return null;
            }

            if (!Int32.TryParse(name.AsSpan(i + 1, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture,
                    out var special))
            {
                return null;
            }

            result.Append(Convert.ToChar(special));
            specialPosition = 4;
        }

        return result.ToString();
    }

    private string GetFolder() => _settings.Folder;

    private void InitializeFolder()
    {
        if (!Directory.Exists(Folder))
        {
            Directory.CreateDirectory(Folder);
        }
    }

    private string GetFileName(string playerListName)
    {
        var result = StringBuilderPoolFactory<FilePlayerListRepository>.Get(nameof(FilePlayerListRepository)) ??
                     new StringBuilder(playerListName.Length + 10);

        result.Clear();

        var invalidCharacters = _invalidCharacters.Value;

        foreach (var c in playerListName)
        {
            
            var isSpecial = Array.BinarySearch(invalidCharacters, c, Comparer<char>.Default) >= 0 
                            || (c != ' ' 
                                && char.IsWhiteSpace(c)
                                );

            if (!isSpecial)
            {
                result.Append(c);
                continue;
            }

            result.Append(SpecialCharacter);
            result.Append((Convert.ToInt32(c)).ToString("X4", CultureInfo.InvariantCulture));
        }

        result.Append(FileExtension);
        return result.ToString();
    }
    #endregion

    public void Dispose()
    {
        if (StringBuilderPoolFactory<FilePlayerListRepository>.Exists(nameof(FilePlayerListRepository)))
        {
            StringBuilderPoolFactory<FilePlayerListRepository>.Remove(nameof(FilePlayerListRepository));
        }
    }
}
