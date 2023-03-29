using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;
public class CrowGameRoundPlayerStatus
{
    public CrowGameRoundPlayerStatus(string memberName, bool hasPlayed)
    {
        if (String.IsNullOrEmpty(memberName))
        {
            throw new ArgumentNullException(nameof(memberName));
        }

        MemberName = memberName;
        HasPlayedWhiteCard = hasPlayed;
    }

    public string MemberName { get; init; }
    public bool HasPlayedWhiteCard { get; init; }
}
