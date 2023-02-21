using TwitchLib.Client.Events;
using TwitchLib.Communication.Events;
using TwitchLib.PubSub.Events;

namespace TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
public interface ITwitchChatService
{
    event EventHandler<OnMessageReceivedArgs>? OnMessageReceived;
    event EventHandler<OnChannelPointsRewardRedeemedArgs>? OnRewardRedeemed;
    event EventHandler<OnJoinedChannelArgs>? OnConnected;
    event EventHandler<OnDisconnectedEventArgs>? OnDisconnected;
    String Username { get; }

    Task ConnectToChannelAsync(String channel, String oauthToken, String username, String clientId,
        CancellationToken cancellationToken = new());

    void SendMessage(String message);
    Boolean IsConnectedToTwitch();
    void DisconnectFromTwitch();
    ValueTask DisconnectFromTwitchAsync();
}
