using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheOmenDen.Shared.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
public sealed record HubEndpointRoutes: EnumerationBase<HubEndpointRoutes>
{
    private HubEndpointRoutes(string name, int id) : base(name, id) {}

    public static readonly HubEndpointRoutes Clear= new(nameof(Clear),1 );
    public static readonly HubEndpointRoutes Connect= new(nameof(Connect),2 );
    public static readonly HubEndpointRoutes Create= new(nameof(Create),3 );
    public static readonly HubEndpointRoutes Join= new(nameof(Join), 4);
    public static readonly HubEndpointRoutes Kick= new(nameof(Kick),5 );
    public static readonly HubEndpointRoutes Show= new(nameof(Show), 6);
    public static readonly HubEndpointRoutes UnSubmit= new(nameof(UnSubmit),7 );
    public static readonly HubEndpointRoutes Submit= new(nameof(Submit),8 );
    public static readonly HubEndpointRoutes ChangePlayerType = new(nameof(ChangePlayerType), 9);
}
