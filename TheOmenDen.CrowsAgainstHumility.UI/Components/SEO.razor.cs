namespace TheOmenDen.CrowsAgainstHumility.Components;

public partial class SEO : ComponentBase
{
    #region Parameters
    [Parameter] public String Title { get; set; }
    [Parameter] public String Description { get; set; }
    [Parameter] public String Canonical { get; set; }
    [Parameter] public String ImageUrl { get; set; }
    #endregion
    #region Injected Members
    [Inject] private NavigationManager NavigationManager { get; init; }
    #endregion
    #region Private Members
    private String _url = String.Empty;
    #endregion
    #region Lifecycle Methods

    protected override void OnInitialized()
    {
        _url = NavigationManager.ToAbsoluteUri(Canonical).AbsoluteUri;

        ImageUrl = String.IsNullOrEmpty(ImageUrl)
            ? NavigationManager.ToAbsoluteUri("images/the-omen-den-logo.png").AbsoluteUri
            : NavigationManager.ToAbsoluteUri(ImageUrl).AbsoluteUri;   

        base.OnInitialized();
    }

    #endregion
}
