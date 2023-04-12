using TheOmenDen.CrowsAgainstHumility.Core.Messages;
using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;
using TheOmenDen.CrowsAgainstHumility.Core.Providers;
using TheOmenDen.CrowsAgainstHumility.Services.Locks;

namespace TheOmenDen.CrowsAgainstHumility.Services.Tasks;
internal class ReceiveMessagesTask
{
    private readonly TaskCompletionSource<IEnumerable<Message>> _taskCompletionSource = new();
    private readonly PlayerListLock _playerListLock;
    private readonly Observer _observer;
    private readonly TaskProvider _taskProvider;

    private volatile bool _isReceivedEventHandlerHooked;

    public ReceiveMessagesTask(PlayerListLock playerListLock, Observer observer, TaskProvider taskProvider)
    {
        _playerListLock = playerListLock;
        _observer = observer;
        _taskProvider = taskProvider;
    }

    public async Task<IEnumerable<Message>> GetMessagesAsync(TimeSpan timeout, CancellationToken cancellationToken)
    {
        using var timeoutCancellationSource = new CancellationTokenSource();
        using var combinedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCancellationSource.Token);

        try
        {
            _observer.MessageReceived += ObserverOnMessageReceived;
            _isReceivedEventHandlerHooked = true;

            var messagesReceivedTask = _taskCompletionSource.Task;
            var timeoutTask = _taskProvider.Delay(timeout, combinedCancellationTokenSource.Token);
            var completedTask = await Task.WhenAny(messagesReceivedTask, timeoutTask);

            cancellationToken.ThrowIfCancellationRequested();

            return completedTask == messagesReceivedTask 
                ? await messagesReceivedTask 
                : Enumerable.Empty<Message>();
        }
        finally
        {
            timeoutCancellationSource.Cancel();

            if (_isReceivedEventHandlerHooked)
            {
                await using var playerListLock = _playerListLock;

                playerListLock.Lock();
                _observer.MessageReceived -= ObserverOnMessageReceived;
                _isReceivedEventHandlerHooked = false;
            }
        }
    }

    private void ObserverOnMessageReceived(object? sender, EventArgs e)
    {
        using var playerListLock = _playerListLock;

        playerListLock.Lock();

        _observer.MessageReceived -= ObserverOnMessageReceived;
        _isReceivedEventHandlerHooked = false;

        IEnumerable<Message> messages = _observer.Messages.ToList();

        _taskCompletionSource.TrySetResult(messages);
    }
}
