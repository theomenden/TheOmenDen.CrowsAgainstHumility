using TheOmenDen.Shared.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
public sealed record ClipboardResults: EnumerationBase<ClipboardResults>
{
    private ClipboardResults(string name, int id) : base(name, id) {}

    public static readonly ClipboardResults Copied = new(nameof(Copied), 1);
    public static readonly ClipboardResults NotCopied = new(nameof(NotCopied), 2);
    public static readonly ClipboardResults Invalid = new(nameof(Invalid), 3);
}
