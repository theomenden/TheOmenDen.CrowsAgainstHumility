using TheOmenDen.Shared.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
public sealed record MessageTypes : EnumerationBase<MessageTypes>
{
    private MessageTypes(string name, int id) : base(name, id)
    {
    }

    public static readonly MessageTypes Empty = new(nameof(Empty), 1);
    public static readonly MessageTypes PlayerJoined = new(nameof(PlayerJoined), 2);
    public static readonly MessageTypes PlayerDisconnected = new(nameof(PlayerDisconnected), 3);
    public static readonly MessageTypes GameRoundStarted = new(nameof(GameRoundStarted), 4);
    public static readonly MessageTypes GameRoundEnded = new(nameof(GameRoundEnded), 5);
    public static readonly MessageTypes GameRoundCanceled = new(nameof(GameRoundCanceled), 6);
    public static readonly MessageTypes PlayerPlayedACard = new(nameof(PlayerPlayedACard), 7);
    public static readonly MessageTypes PlayerActivity = new(nameof(PlayerActivity), 8);
    public static readonly MessageTypes PlayerListCreated = new(nameof(PlayerListCreated), 9);
    public static readonly MessageTypes AvailableCardsChanged = new(nameof(AvailableCardsChanged), 10);
    public static readonly MessageTypes TimerStarted = new(nameof(TimerStarted), 11);
    public static readonly MessageTypes TimerCanceled = new(nameof(TimerCanceled), 12);
}
