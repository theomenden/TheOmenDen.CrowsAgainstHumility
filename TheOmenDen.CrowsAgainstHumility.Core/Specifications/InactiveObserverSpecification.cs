using System.Linq.Expressions;
using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;
using TheOmenDen.Shared.Specifications;

namespace TheOmenDen.CrowsAgainstHumility.Core.Specifications;
public sealed record InactiveObserverSpecification(DateTime LastInactivityTime): Specification<Observer>
{
    public override Expression<Func<Observer, bool>> ToExpression()
        => observer => observer.LastActivity < LastInactivityTime 
                       && observer.IsDormant;
}
