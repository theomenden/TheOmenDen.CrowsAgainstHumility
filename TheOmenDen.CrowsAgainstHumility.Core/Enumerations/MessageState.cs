using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheOmenDen.Shared.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
public sealed record MessageState: EnumerationBase<MessageState, byte>
{
    private MessageState(string name, byte id) : base(name, id)
    {}

    public static readonly MessageState Normal = new (nameof(Normal),0);
    public static readonly MessageState Queued = new (nameof(Queued), 1);
    public static readonly MessageState Failed = new (nameof(Failed), 2);
}
