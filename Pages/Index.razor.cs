using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using TheOmenDen.CrowsAgainstHumility.Circuits;
using TheOmenDen.CrowsAgainstHumility.Core.Rules;
using TheOmenDen.CrowsAgainstHumility.Data.Contexts;
using TheOmenDen.CrowsAgainstHumility.Events;
using TheOmenDen.CrowsAgainstHumility.Services.CrowGameBuilder;
using TheOmenDen.CrowsAgainstHumility.Services.Interfaces;

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

    [Inject]
    public IDbContextFactory<CrowsAgainstHumilityContext> DbContextFactory { get; init; }

    [Inject]
    public IPlayerVerificationService VerificationService { get; init; }

    #endregion

    private TrackingCircuitHandler _trackingCircuitHandler;

    private string _sessionCircuitMessage = String.Empty;

    private IEnumerable<String> _packNames = Enumerable.Empty<String>();

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

        await using var context = await DbContextFactory.CreateDbContextAsync();

        _packNames = await context.Packs
            .Where(p => p.IsOfficialPack)
            .Select(p => p.Name)
            .ToArrayAsync();

        await VerificationService.CheckTwitchForUser("aluthecrow");
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
