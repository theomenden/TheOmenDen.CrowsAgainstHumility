using TheOmenDen.CrowsAgainstHumility.Azure.Clients;
using TheOmenDen.CrowsAgainstHumility.Core.DTO.ViewModels;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Components.Game;

public partial class PlayerList : ComponentBase
{
    #region Parameters
    [Parameter] public Guid Id { get; set; }
    [Parameter] public EventCallback<Guid> IdChanged { get; set; }
    [Parameter] public ICrowGameHubClient HubClient { get; set; }
    [Parameter] public EventCallback<ICrowGameHubClient> HubClientChanged { get; set; }
    [Parameter] public PlayerViewModel CurrentPlayer { get; set; }
    [Parameter] public EventCallback<PlayerViewModel> CurrentPlayerChanged { get; set; }
    [Parameter] public CrowGameServerViewModel Server { get; set; }
    [Parameter] public EventCallback<CrowGameServerViewModel> ServerChanged { get; set; }
    #endregion

    #region Private Methods

    private Task KickPlayerAsync(int playerPublicId) => HubClient.KickPlayer(Id, CurrentPlayer.ConnectionId, playerPublicId);

    private async Task ChangePlayerRole(GameRoles newRole)
    {
        var updatedPlayer = await HubClient.ChangePlayerType(Id, newRole);

        await CurrentPlayerChanged.InvokeAsync(updatedPlayer);
    }

    private bool PlayerHasPlayedACard() =>
        Server.CurrentSession.PlayedCards.Any(v => v.Key == CurrentPlayer.PublicId.ToString());
    #endregion
}
