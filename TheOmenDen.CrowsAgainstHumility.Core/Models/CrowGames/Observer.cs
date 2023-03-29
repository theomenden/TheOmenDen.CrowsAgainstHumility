using System.Reflection.Metadata.Ecma335;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Messages;
using TheOmenDen.CrowsAgainstHumility.Core.Results;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;
public class Observer
{
    private readonly Queue<Message> _messages = new();
    private long _lastMessageId;

    public Observer(PlayerList players, string name)
    {
        if (players is null)
        {
            throw new ArgumentNullException(nameof(players));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException(nameof(name));
        }


    }

    public event EventHandler? MessageReceived;

    public PlayerList Players { get; private set; }
    public string Name { get; private set; }
    public bool HasMessage => _messages.Count > 0;
    public IEnumerable<Message> Messages => _messages;
    public Guid SessionId { get; set; }
    public long AckowledgedMessageId { get; private set; }
    public DateTime LastActivity { get; private set; }
    public bool IsDormant { get; internal set; }

    public void AcknowledgeMessages(Guid sessionId, long lastMessageId)
    {
        if (sessionId == Guid.Empty || sessionId != SessionId)
        {
            throw new ArgumentException(nameof(sessionId));
        }

        if (lastMessageId <= AckowledgedMessageId)
        {
            return;
        }

        AckowledgedMessageId = lastMessageId;

        while (HasMessage && _messages.Peek().Id <= lastMessageId)
        {
            _messages.Dequeue();
        }
    }

    public long ClearMessage()
    {
        _messages.Clear();
        AckowledgedMessageId = _lastMessageId;
        return _lastMessageId;
    }

    public void UpdateActivity()
    {
        IsDormant = false;
        LastActivity = Players.DateTimeProvider.UtcNow;
        PlayerList.OnObserverActivity(this);
    }

    #region Internal Methods
    internal void SendMessage(Message message)
    {
        if (message is null)
        {
            return;
        }

        _lastMessageId++;
        message.Id = _lastMessageId;
        _messages.Enqueue(message);
        OnMessageReceived(System.EventArgs.Empty);
    }

    internal void DeserializeMessage(Serialization.PlayerData memberData)
    {
        if (memberData.Messages is null)
        {
            return;
        }

        foreach (var messageData in memberData.Messages)
        {
            _messages.Enqueue(CreateMessage(messageData));
        }
    }

    protected internal virtual Serialization.PlayerData GetData()
    {
        var result = new Serialization.PlayerData
        {
            Name = Name,
            MemberType = GameRoles.Observer,
            LastActivity = LastActivity,
            LastMessageId = _lastMessageId,
            SessionId = SessionId,
            IsDormant = IsDormant
        };

        return result;
    }
    #endregion
    #region Private Messages

    private Message CreateMessage(Serialization.MessageData messageData)
    {
        Message? message = null;

        messageData.MessageType
            .When(MessageTypes.Empty, MessageTypes.GameRoundStarted, MessageTypes.GameRoundCanceled, MessageTypes.TimerCanceled)
                .Then(() =>
                {
                    message = new Message(messageData);
                })
            .When(MessageTypes.PlayerJoined, MessageTypes.PlayerDisconnected, MessageTypes.PlayerPlayedACard)
                .Then(() =>
                {
                    var memberName = messageData.PlayerName!;
                    var member = PlayerList.FindPlayerOrObserver(memberName) ?? new Player(Players, memberName);
                    message = new PlayerMessage(messageData, member);
                })
            .When(MessageTypes.GameRoundEnded)
                .Then(() =>
                {
                    var roundResult = new RoundResult(Players, messageData.PlayedResult!);
                    roundResult.SetReadOnly();
                    message = new RoundResultMessage(messageData, roundResult);
                })
            .When(MessageTypes.AvailableCardsChanged)
                .Then(() => message = new WhiteCardSetMessage(messageData))
            .When(MessageTypes.TimerStarted)
                .Then(() => message = new TimerMessage(messageData))
            .DefaultCondition(() => throw new ArgumentException($"Invalid message type {messageData.MessageType}.", nameof(messageData)));

        return message!;
    }
    #endregion
}
