using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.JSInterop;
using TheOmenDen.CrowsAgainstHumility.Circuits;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
using TheOmenDen.CrowsAgainstHumility.Events;

namespace TheOmenDen.CrowsAgainstHumility.Pages;
public partial class Index : ComponentBase, IDisposable, IAsyncDisposable
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

    [Inject] public ISessionDetails SessionDetails { get; init; }

    [Inject]
    public CircuitHandler CircuitHandler { get; init; }

    [Inject] protected IUserService UserService { get; init; }

    [Inject] private IUserInformationService UserInformationService { get; init; }

    [Inject] private ILogger<Index> Logger { get; init; }
    #endregion

    private TrackingCircuitHandler _trackingCircuitHandler;

    private string _sessionCircuitMessage = String.Empty;

    private bool _isDisposed;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        try
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
        catch (Exception ex)
        {
            Logger.LogError("Failed to initialize the index due to an exception @{Ex}", ex);
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


    public ValueTask DisposeAsync()
    {
        SessionDetails.CircuitsChanged -= OnCircuitsChanged;
        SessionDetails.UserDisconnect -= OnUserDisconnected;
        AuthenticationStateTask.Dispose();
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }


    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed)
        {
            return;
        }

        if (disposing)
        {
            SessionDetails.CircuitsChanged -= OnCircuitsChanged;
            SessionDetails.UserDisconnect -= OnUserDisconnected;
        }

        AuthenticationStateTask.Dispose();
        _isDisposed = true;
    }
}
