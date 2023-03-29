using TheOmenDen.Shared.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
public sealed record PlayerListState : EnumerationBase<PlayerListState>
{
    private PlayerListState(string name, int id) : base(name, id)
    {
    }

    public static readonly PlayerListState Initial = new(nameof(Initial), 1);
    public static readonly PlayerListState RoundInProgress = new(nameof(RoundInProgress), 2);
    public static readonly PlayerListState RoundFinished = new(nameof(RoundFinished), 3);
    public static readonly PlayerListState RoundCanceled = new(nameof(RoundCanceled), 4);
}
