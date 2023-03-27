namespace TheOmenDen.CrowsAgainstHumility.Services;

public interface IClipboardService
{
    ValueTask CopyToClipboardAsync(String text, CancellationToken cancellationToken = default);
}
