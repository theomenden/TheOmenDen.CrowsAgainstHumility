using TheOmenDen.Shared.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

public sealed record PlayerMode : EnumerationBase<PlayerMode>
{
    private PlayerMode(string name, int id) : base(name, id)
    {
    }

    public static readonly PlayerMode Awake = new(nameof(Awake), 1);
    public static readonly PlayerMode Asleep = new(nameof(Asleep), 2);
}