using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.JSInterop;
using TheOmenDen.CrowsAgainstHumility.Circuits;
using TheOmenDen.CrowsAgainstHumility.Events;

namespace TheOmenDen.CrowsAgainstHumility.Pages;
public partial class Index : IDisposable, IAsyncDisposable
{
    #region Parameters
    [CascadingParameter]
    public Task<AuthenticationState> AuthenticationStateTask { get; set; }
    #endregion
    #region Injected Services
    [Inject]
    public IJSRuntime JSRuntime { get; init; }

    [Inject]
    public NavigationManager NavigationManager { get; init; }

    [Inject]
    public ISessionDetails SessionDetails { get; init; }

    [Inject]
    public CircuitHandler CircuitHandler { get; init; }
    #endregion

    private TrackingCircuitHandler _trackingCircuitHandler;

    private string _sessionCircuitMessage = String.Empty;

    protected override async Task OnInitializedAsync()
    {
        _trackingCircuitHandler = (TrackingCircuitHandler)CircuitHandler;
        _sessionCircuitMessage = $"My Circuit ID = {_trackingCircuitHandler.CircuitId}";

        var authState = await AuthenticationStateTask;

        var userIdentity = authState.User.Identity;

        if (userIdentity is { IsAuthenticated: true })
        {
            SessionDetails.ConnectSession(_trackingCircuitHandler.CircuitId, userIdentity);
            SessionDetails.CircuitsChanged += OnCircuitsChanged;
            SessionDetails.UserDisconnect += OnUserDisconnected;
        }
    }

    private void OnUserDisconnected(object sender, UserDisconnectEventArgs e)
    {
        InvokeAsync(StateHasChanged);
    }

    private void OnCircuitsChanged(object? sender, EventArgs e)
    {
        InvokeAsync(StateHasChanged);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync("registerBlazorApp", $"{NavigationManager.BaseUri}Identity/Account/Login");
        }
    }

    public void Dispose()
    {
        SessionDetails.CircuitsChanged -= OnCircuitsChanged;
        SessionDetails.UserDisconnect -= OnUserDisconnected;
        GC.SuppressFinalize(this);
    }

    public ValueTask DisposeAsync()
    {
        SessionDetails.CircuitsChanged -= OnCircuitsChanged;
        SessionDetails.UserDisconnect -= OnUserDisconnected;
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}
