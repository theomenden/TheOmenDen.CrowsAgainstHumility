using Microsoft.JSInterop;

namespace TheOmenDen.CrowsAgainstHumility.Services;

public sealed class ClipboardService: IClipboardService
{
    private readonly IJSRuntime _jsInterop;
    private readonly ILogger<ClipboardService> _logger;

    public ClipboardService(IJSRuntime jsInterop, ILogger<ClipboardService> logger)
    {
        _jsInterop = jsInterop;
        _logger = logger;
    }

    public ValueTask CopyToClipboardAsync(String text, CancellationToken cancellationToken = default)
        => _jsInterop.InvokeVoidAsync("navigator.clipboard.writeText", cancellationToken, text);
}
