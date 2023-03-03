using System.Runtime.CompilerServices;
using TheOmenDen.CrowsAgainstHumility.Core.Models.ChatModels;

namespace TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
public interface IBetterTTVEmoteService
{
    IAsyncEnumerable<BttvBaseEmote> GetBetterTtvGlobalEmotesAsync(CancellationToken cancellationToken = default);
}
