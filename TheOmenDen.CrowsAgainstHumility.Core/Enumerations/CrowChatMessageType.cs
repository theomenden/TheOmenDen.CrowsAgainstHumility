using TheOmenDen.Shared.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
public sealed record CrowChatMessageType: EnumerationBase<CrowChatMessageType>
{
    private CrowChatMessageType(String messageType, Int32 id)
    : base(messageType, id)
    {
    }

    public static readonly CrowChatMessageType GameFlow = new(nameof(GameFlow), 1);
    public static readonly CrowChatMessageType Chat = new(nameof(Chat), 2);
    public static readonly CrowChatMessageType WhiteCardPlay = new(nameof(WhiteCardPlay), 3);
    public static readonly CrowChatMessageType WinningCardChosen = new(nameof(WinningCardChosen), 4);
    public static readonly CrowChatMessageType BlackCardPlayed = new(nameof(BlackCardPlayed), 5);
    public static readonly CrowChatMessageType AlreadyPlayedCard = new(nameof(AlreadyPlayedCard), 6);
}
