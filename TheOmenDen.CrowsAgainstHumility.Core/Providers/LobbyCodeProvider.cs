using System.Security.Cryptography;
using System.Text;
using TheOmenDen.Shared.Extensions;

namespace TheOmenDen.CrowsAgainstHumility.Core.Providers;
public sealed class LobbyCodeProvider
{
    public static LobbyCodeProvider Default { get; } = new();

    private const string LowerCaseAlphabet = @"abcdefghijklmnopqrstuvwxyz";
    private const string UpperCaseAlphabet = @"ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Digits = @"1234567890";
    private const string Discriminators = @"-_";
    private static readonly char[] FullRange = $"{LowerCaseAlphabet}{UpperCaseAlphabet}{Digits}{Discriminators}".ToCharArray();

    public static string GenerateGameCodeFromComponent(String componentName)
    {
        var sb = StringBuilderPoolFactory<LobbyCodeProvider>.Create(componentName);

        sb.Clear();

        return GenerateGameCode(sb).ToString();
    }

    private static StringBuilder GenerateGameCode(StringBuilder sb)
    {
        var data = new byte[32];

        using var crypto = RandomNumberGenerator.Create();

        crypto.GetBytes(data);

        for (var i = 0; i < 8; i++)
        {
            var random = BitConverter.ToUInt32(data, i * 4);

            var index = random % FullRange.Length;

            sb.Append(FullRange[index]);
        }

        return sb;
    }
}
