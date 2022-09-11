using Microsoft.JSInterop;

namespace TheOmenDen.CrowsAgainstHumility.Pages;
public partial class Index
{
    [Inject]
    public IJSRuntime JSRuntime { get; init; }

    [Inject]
    public NavigationManager NavigationManager { get; init; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync("registerBlazorApp", $"{NavigationManager.BaseUri}Identity/Account/Login");
        }
    }
}
