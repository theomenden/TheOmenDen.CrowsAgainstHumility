namespace TheOmenDen.CrowsAgainstHumility.Providers;

public class CrowGameServerUriProvider
{
    public Uri? BaseUri { get; private set; }

    public void InitializeBaseUri(Uri baseUri) => BaseUri = baseUri;
}
