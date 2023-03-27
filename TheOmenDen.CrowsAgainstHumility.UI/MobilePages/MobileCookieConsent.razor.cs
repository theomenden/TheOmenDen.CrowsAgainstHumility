using Microsoft.JSInterop;

namespace TheOmenDen.CrowsAgainstHumility.MobilePages;

public partial class MobileCookieConsent: ComponentBase
{
    [Inject] private IJSRuntime JSRuntime { get; init; }
    [Inject] private IConfiguration Config { get; init; }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync("loadJs", Config["Cookies:CookieConsent"]);
        }
    }
}
