using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;

namespace TheOmenDen.CrowsAgainstHumility.Services.Helpers;
internal sealed class DateTimeProvider: IDateTimeProvider
{
    public DateTime GetUtcNow() => DateTime.UtcNow;
}
