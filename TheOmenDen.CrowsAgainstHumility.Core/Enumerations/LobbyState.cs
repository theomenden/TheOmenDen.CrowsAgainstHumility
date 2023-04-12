using TheOmenDen.Shared.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
public sealed record LobbyState : EnumerationBase<LobbyState>
{
    private LobbyState(string name, int id) : base(name, id)
    {
    }

    public static readonly LobbyState Initial = new(nameof(Initial), 1);
    public static readonly LobbyState RoundInProgress = new(nameof(RoundInProgress), 2);
    public static readonly LobbyState RoundFinished = new(nameof(RoundFinished), 3);
    public static readonly LobbyState RoundCanceled = new(nameof(RoundCanceled), 4);
}
