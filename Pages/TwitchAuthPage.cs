using System.Configuration;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TwitchLib.Api;
using TwitchLib.Api.Auth;
using TwitchLib.Api.Core;
using TwitchLib.Api.Interfaces;

namespace TheOmenDen.CrowsAgainstHumility.Pages
{
    public partial class TwitchAuthPage: ComponentBase //, IAsyncDisposable
    {
        [Inject]public TwitchStrings TwitchStrings { get; init; }
        
        private TwitchAPI _twitchAPI;
        private List<String> _userNames;

        protected override async Task OnInitializedAsync()
        {
            var settings = new ApiSettings
            {
                ClientId = TwitchStrings.ClientId,
                Secret = TwitchStrings.Key
            };

            _twitchAPI = new(settings: settings);
            
            var resp = await _twitchAPI.Auth.GetAccessTokenFromCodeAsync(auth.Code, Config.TwitchClientSecret, Config.TwitchRedirectUri);

            _twitchAPI.Settings.AccessToken = resp.AccessToken;
            var users = (await _twitchAPI.Helix.Users.GetUsersAsync()).Users;

            _userNames = users.Select(u => u.DisplayName).ToList();
        }

    }
}
