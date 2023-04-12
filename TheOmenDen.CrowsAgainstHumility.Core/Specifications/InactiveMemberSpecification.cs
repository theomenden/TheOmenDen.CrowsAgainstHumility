using System.Linq.Expressions;
using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;
using TheOmenDen.Shared.Specifications;

namespace TheOmenDen.CrowsAgainstHumility.Core.Specifications;
public sealed record InactiveMemberSpecification(DateTime LastInactivityTime) : Specification<Member>
{
    public override Expression<Func<Member, bool>> ToExpression()
        => observer => observer.LastActivity < LastInactivityTime
                       && observer.IsDormant;
}