using System.Security.Cryptography;
using TheOmenDen.Shared.Extensions;

namespace TheOmenDen.CrowsAgainstHumility.Services.Authentication;

/// <summary>
/// This class generates a (pseudo)hashed game code to allow players to securely connect to a game.
/// </summary>
public sealed record GameCodeGenerator
{
    private const string LowerCaseAlphabet = @"abcdefghijklmnopqrstuvwxyz";
    private const string UpperCaseAlphabet = @"ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Digits = @"1234567890";
    private const string Discriminators = @"-_";
    private static readonly Char[] FullRange = $"{LowerCaseAlphabet}{UpperCaseAlphabet}{Digits}{Discriminators}".ToCharArray();
    
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