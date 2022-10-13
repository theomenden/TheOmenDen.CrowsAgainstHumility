namespace TheOmenDen.CrowsAgainstHumility.Twitch.Interfaces;
public interface ITwitchRedemptionsManager : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Configures access to the <paramref name="channel"/> for managing channel redeems and such
    /// </summary>
    /// <param name="channel">The channel we want to connect to</param>
    /// <param name="oauthToken">An OAuth token</param>
    /// <param name="username">The username that's associated with this connection</param>
    /// <param name="clientId">The client id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task ConnectToChannelAsync(String channel, String oauthToken, String username, String clientId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies that we're connected to twitch
    /// </summary>
    /// <returns><see langword="true"/> when connected; <see langword="false"/> otherwise</returns>
    Boolean IsConnectedToTwitch();

    /// <summary>
    /// Sends a message to the twitch channel we're connected to.
    /// </summary>
    /// <param name="message">The message we want to send out</param>
    void SendMessage(String message);

    /// <summary>
    /// The username that's connecting to the channel
    /// </summary>
    /// <value>A string representation</value>
    String Username { get; }
}
