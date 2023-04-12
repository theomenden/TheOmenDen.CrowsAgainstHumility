using System.Linq.Expressions;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.Shared.Specifications;

namespace TheOmenDen.CrowsAgainstHumility.Core.Specifications;
public sealed record OfficialPacksSpecification: Specification<Pack>
{
    public override Expression<Func<Pack, bool>> ToExpression()
    => pack => pack.IsOfficialPack;
}
