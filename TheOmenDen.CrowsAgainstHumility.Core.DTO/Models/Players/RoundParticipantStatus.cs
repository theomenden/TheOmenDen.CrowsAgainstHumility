namespace TheOmenDen.CrowsAgainstHumility.Core.DTO.Models.Players;
public class RoundParticipantStatus
{
    public RoundParticipantStatus(string memberName, bool hasPlayed)
    {
        ArgumentNullException.ThrowIfNull(memberName);
        MemberName = memberName;
        PlayedCard = hasPlayed;
    }

    public string MemberName { get; private set; }
    public bool PlayedCard { get; private set; }
}
