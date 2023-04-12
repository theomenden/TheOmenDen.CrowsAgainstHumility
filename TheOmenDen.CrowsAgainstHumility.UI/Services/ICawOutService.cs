namespace TheOmenDen.CrowsAgainstHumility.Services;

internal interface ICawOutService: IAsyncDisposable
{
    ValueTask<string> ShareOnSocialMedia(string title, string text, string url);
    ValueTask<bool> CanShareOnSocialMedia();
}