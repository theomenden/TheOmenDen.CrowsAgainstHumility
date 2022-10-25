namespace TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
public interface IMessageChannelService<T>
{
    ValueTask PublishMessageAsync(T content, CancellationToken cancellationToken = default);
    ValueTask<T> ReceiveMessageAsync(CancellationToken cancellationToken = default);
    IAsyncEnumerable<T> GetAllMessagesAsync(CancellationToken cancellationToken = default);
}