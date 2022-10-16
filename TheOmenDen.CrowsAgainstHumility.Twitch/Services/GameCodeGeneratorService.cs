using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TheOmenDen.Shared.Extensions;
using TheOmenDen.Shared.Utilities;

namespace TheOmenDen.CrowsAgainstHumility.Twitch.Services;
public sealed class GameCodeGeneratorService
{
    private const string LowerCaseAlphabet = @"abcdefghijklmnopqrstuvwxyz";
    private const string UpperCaseAlphabet = @"ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Digits = @"1234567890";
    private const string Discriminators = @"-_";
    private static readonly Char[] FullRange = $"{LowerCaseAlphabet}{UpperCaseAlphabet}{Digits}{Discriminators}".ToCharArray();

    public GameCodeGeneratorService()
    {
    }

    public static string GenerateGameCode()
    {
        var data = new byte[32];

        using var crypto = RandomNumberGenerator.Create();

        crypto.GetBytes(data);

        var sb = StringBuilderPoolFactory<GameCodeGeneratorService>.Create(nameof(GameCodeGeneratorService));

        for (var i = 0; i < 8; i++)
        {
            var random = BitConverter.ToUInt32(data, i * 4);

            var index = random % FullRange.Length;

            sb.Append(FullRange[index]);
        }

        return sb.ToString();
    }

}
