using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Cards;
using TheOmenDen.Shared.Interfaces.Accessors;
namespace TheOmenDen.CrowsAgainstHumility.Data.Repositories;

public interface IPackRepository: IAccessor<Pack>, IAsyncStreamAccessor<Pack>, IEnumerable<Pack>, IAsyncEnumerable<Pack>
{
}