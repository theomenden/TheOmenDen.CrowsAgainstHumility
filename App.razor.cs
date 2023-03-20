using Blazorise;
namespace TheOmenDen.CrowsAgainstHumility;

public partial class App : IDisposable, IAsyncDisposable
{
    #region Private Members
    private readonly Theme _theme = new()
    {
        IsGradient = true,
        LuminanceThreshold = 170,
        ColorOptions = new ThemeColorOptions
        {
            Primary = "#7A04AB",
            Secondary = "#FF00A0",
            Dark = "#120458",
            Light = "#FE75FE",
            Link = "#cfe2f3",
            Warning = "#E2C709"
        },
        BackgroundOptions = new ThemeBackgroundOptions
        {
            Primary = "#FF00A0",
            Secondary = "#7A04AB",
            Dark = "#030011",
            Light = "#FE75FE",
            Warning = "#E2C709",
        },
        BarOptions = new ThemeBarOptions
        {
            DarkColors = new ThemeBarColorOptions
            {
                BackgroundColor = "#030011",
                Color = "#ffffff",
                ItemColorOptions = new ThemeBarItemColorOptions
                {
                    ActiveBackgroundColor = "#FE42FE",
                    HoverBackgroundColor = "#7A04AB",
                    ActiveColor = "#000000",
                    HoverColor = "#ffffff"
                }
            }
        }
    };
    #endregion
    #region Parameters
    [Parameter] public bool IsMobile { get; set; }
    [Parameter] public ApplicationUserState? InitialState { get; set; }
    #endregion
    #region Injected Members
    [Inject] private NavigationManager NavigationManager { get; init; }
    [Inject] private TokenProvider TokenProvider { get; init; }
    #endregion
    #region Location Event Methods
    private void OnLocationChanged(object? sender, EventArgs eventArgs) => NavCheck();

    private void NavCheck()
    {
        var navPath = new Uri(NavigationManager.Uri).LocalPath.ToLower();

        if (!IsMobile || navPath.StartsWith("/mobile"))
        {
            return;
        }

        var newPath = $"/mobile/{navPath.Substring(1)}";
        NavigationManager.NavigateTo(newPath);
    }
    #endregion
    #region Lifecycle Methods
    protected override void OnInitialized()
    {
        base.OnInitialized();
        NavigationManager.LocationChanged += OnLocationChanged;
        NavCheck();
    }

    protected override Task OnInitializedAsync()
    {
        TokenProvider.XSRFToken = InitialState?.Xsrf ?? String.Empty;
        TokenProvider.AccessToken = InitialState?.AccessToken ?? String.Empty;
        TokenProvider.RefreshToken = InitialState?.RefreshToken ?? String.Empty;
        TokenProvider.Username = InitialState?.Username ?? String.Empty;
        TokenProvider.IsAuthenticated = InitialState?.IsAuthenticated ?? false;
        return base.OnInitializedAsync();
    }
    #endregion
    #region Disposal Methods
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            NavigationManager.LocationChanged -= OnLocationChanged;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public ValueTask DisposeAsync()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
    #endregion
}
