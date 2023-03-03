using Microsoft.JSInterop;

namespace TheOmenDen.CrowsAgainstHumility.Pages;

public partial class CookieConsent: ComponentBase
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
