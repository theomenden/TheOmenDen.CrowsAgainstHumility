namespace TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
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
    /// The username that's connecting to the channel
    /// </summary>
    /// <value>A string representation</value>
    String Username { get; }
}
