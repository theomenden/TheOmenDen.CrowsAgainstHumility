using System.Security.Cryptography;

namespace TheOmenDen.CrowsAgainstHumility.Services;

public sealed class GameCodeGenerator
{
    private const string LowerCaseAlphabet = @"abcdefghijklmnopqrstuvwxyz";
    private const string UpperCaseAlphabet = @"ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Digits = @"1234567890";
    private const string Discriminators = @"-_";
    private static readonly Char[] FullRange = $"{LowerCaseAlphabet}{UpperCaseAlphabet}{Digits}{Discriminators}".ToCharArray();

    public static string GenerateGameCodeFromComponent(String componentName)
    {
        var data = new byte[32];

        using var crypto = RandomNumberGenerator.Create();

        crypto.GetBytes(data);

        var sb = StringBuilderPoolFactory<GameCodeGenerator>.Create(componentName);

        for (var i = 0; i < 8; i++)
        {
            var random = BitConverter.ToUInt32(data, i * 4);

            var index = random % FullRange.Length;

            sb.Append(FullRange[index]);
        }

        return sb.ToString();
    }

    public static string GenerateGameCode()
    {
        var data = new byte[32];

        using var crypto = RandomNumberGenerator.Create();

        crypto.GetBytes(data);

        var sb = StringBuilderPoolFactory<GameCodeGenerator>.Create(nameof(GameCodeGenerator));

        for (var i = 0; i < 8; i++)
        {
            var random = BitConverter.ToUInt32(data, i * 4);

            var index = random % FullRange.Length;

            sb.Append(FullRange[index]);
        }

        return sb.ToString();
    }
}
