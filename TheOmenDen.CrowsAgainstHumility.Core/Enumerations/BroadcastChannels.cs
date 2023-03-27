using TheOmenDen.Shared.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
public sealed record BroadcastChannels: EnumerationBase<BroadcastChannels>
{
    private BroadcastChannels(string name, int id) : base(name, id) {}

    public static readonly BroadcastChannels Clear = new("CLEAR", 1);
    public static readonly BroadcastChannels Updated = new("UPDATED", 2);
    public static readonly BroadcastChannels Log = new("LOG", 3);
    public static readonly BroadcastChannels Kicked = new("KICKED", 4);
}
