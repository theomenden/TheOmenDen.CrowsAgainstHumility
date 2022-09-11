using Fluxor;

namespace TheOmenDen.CrowsAgainstHumility.Store;
[FeatureState]
public class CrowGameState
{
    public bool IsLoading { get; }

    private CrowGameState()
    {
    }

    public CrowGameState(bool isLoading)
    {
        IsLoading = isLoading;
    }
}