using Microsoft.JSInterop;

namespace TheOmenDen.CrowsAgainstHumility.Services;

internal sealed class CawOutService : ICawOutService
{
    private readonly Lazy<Task<IJSObjectReference>> _moduleTask;

    public CawOutService(IJSRuntime jsRuntime)
    {
        ArgumentNullException.ThrowIfNull(jsRuntime);

        _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/TheOmenDen.CrowsAgainstHumility/crowsAgainstSocialsScript.min.js"
            ).AsTask()
        );
    }

    public async ValueTask<string> ShareOnSocialMedia(string title, string text, string url)
    {
        var module = await _moduleTask.Value;

        return await module.InvokeAsync<string>("cawOutOnSocialMedia", title, text, url);
    }

    public async ValueTask<bool> CanShareOnSocialMedia()
    {
        var module = await _moduleTask.Value;

        return await module.InvokeAsync<bool>("canCawOutOnSocialMedia");
    }

    public async ValueTask DisposeAsync()
    {
        if (!_moduleTask.IsValueCreated)
        {
            await ValueTask.CompletedTask;
            return;
        }

        var module = await _moduleTask.Value;
        await module.DisposeAsync();
    }
}
