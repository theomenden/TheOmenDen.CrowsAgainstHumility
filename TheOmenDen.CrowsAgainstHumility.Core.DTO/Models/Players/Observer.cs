using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Identity;
using TheOmenDen.CrowsAgainstHumility.Core.DTO.Messages.Domain;
using TheOmenDen.CrowsAgainstHumility.Core.DTO.Results.Domain;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using Message = TheOmenDen.CrowsAgainstHumility.Core.DTO.Messages.Domain.Message;
using RoundResultMessage = TheOmenDen.CrowsAgainstHumility.Core.DTO.Messages.Domain.RoundResultMessage;

namespace TheOmenDen.CrowsAgainstHumility.Core.DTO.Models.Players;
public class Observer
{
    #region Private Members
    private readonly Queue<Message> _messages = new ();
    private long _lastRecievedMessageId;
    #endregion
    #region Constructors
    public Observer(Lobby lobby, string name)
    {
        ArgumentNullException.ThrowIfNull(lobby);
        ArgumentException.ThrowIfNullOrEmpty(name);
        Lobby = lobby;
        Name = name;
        LastActiveAt = Lobby.DateTimeProvider.UtcNow;
    }

    internal Observer(Lobby lobby, Serializations.MemberData memberData)
    {
        ArgumentException.ThrowIfNullOrEmpty(memberData.Name);

        Lobby = lobby;
        Name= memberData.Name;
        LastActiveAt = DateTime.SpecifyKind(memberData.LastActiveAt, DateTimeKind.Utc);
        _lastRecievedMessageId = memberData.LastMessageId;
        SessionId = memberData.SessionId;
        IsDormant = memberData.IsDormant;
    }
    #endregion
    #region Public Properties
    public event EventHandler? MessageReceived;
    public Lobby Lobby { get; private set; }
    public string Name { get; private set; }
    public DateTime LastActiveAt { get; private set; }
    public long LastAcknowledgedMessageId { get; private set; }
    public bool IsDormant { get; internal set; }
    public bool HasMessage => _messages.Count > 0;
    public IEnumerable<Message> Messages => _messages;
    /// <summary>
    /// Links to the <see cref="ApplicationUser"/>
    /// </summary>
    public Guid RecoveryId { get; set; }
    public Guid SessionId { get; set; }
    public int PublicId { get; set; }
    public PlayerMode Mode { get; set; } = PlayerMode.Awake;
    public GameRoles Role { get; set; } = GameRoles.Observer;
    #endregion
    #region Public Methods
    public void AcknowledgeMessages(Guid sessionId, long lastMessageId)
    {
        if (sessionId == Guid.Empty || sessionId != SessionId)
        {
            throw new ArgumentException(Resources.Resources.InvalidSessionId, nameof(sessionId));
        }

        if (lastMessageId <= LastAcknowledgedMessageId)
        {
            return;
        }

        LastAcknowledgedMessageId = lastMessageId;

        while (HasMessage && _messages.Peek().Id <= lastMessageId)
        {
            _messages.Dequeue();
        }
    }

    public long ClearMessagesInQueue()
    {
        _messages.Clear();
        LastAcknowledgedMessageId = _lastRecievedMessageId;
        return _lastRecievedMessageId;
    }

    public void UpdateActivity()
    {
        IsDormant = false;
        Mode = PlayerMode.Awake;
        LastActiveAt = Lobby.DateTimeProvider.UtcNow;
        Lobby.OnObserverActivity(this);
    }
    #endregion
    #region Internal Methods
    internal void SendMessage(Message message)
    {
        if (message is null)
        {
            return;
        }

        _lastRecievedMessageId++;
        message.Id = _lastRecievedMessageId;
        _messages.Enqueue(message);
        OnMessageReceived(EventArgs.Empty);
    }

    internal void DeserializeMessages(Serializations.MemberData memberData)
    {
        if (memberData.Messages?.Any() == true)
        {
            return;
        }

        foreach (var messageData in memberData.Messages)
        {
            _messages.Enqueue(CreateMessage(messageData));
        }
    }

    protected internal virtual Serializations.MemberData GetData()
    {
        var result = new Serializations.MemberData
        {
            Name = Name,
            GameRole = GameRoles.Observer,
            LastActiveAt = LastActiveAt,
            LastMessageId = _lastRecievedMessageId,
            SessionId = SessionId,
            IsDormant = IsDormant
        };

        result.Messages = Messages.Select(m => m.GetData()).ToList();

        return result;
    }

    protected virtual void OnMessageReceived(EventArgs e)
    {
        MessageReceived?.Invoke(this, e);
    }
    #endregion
    #region Private Methods
    private Message CreateMessage(Serializations.MessageData messageData)
    {
        Message? message = null;

        messageData.MessageType
            .When(MessageTypes.Empty, MessageTypes.GameRoundStarted, MessageTypes.GameRoundCanceled, MessageTypes.TimerCanceled)
                .Then(() => message = new Message(messageData))
            .When(MessageTypes.PlayerJoined, MessageTypes.PlayerDisconnected, MessageTypes.PlayerPlayedACard)
                .Then(() =>
                {
                    var memberName = messageData.MemberName!;
                    var member = Lobby.FindMemberOrObserver(memberName) ?? new Member(Lobby, memberName);

                    message = new MemberMessage(messageData, member);
                })
            .When(MessageTypes.GameRoundEnded)
                .Then(() =>
                {
                    var roundResult = new RoundResult(Lobby, messageData.PlayedCardResult!);
                    roundResult.SetToReadOnly();
                    message = new RoundResultMessage(messageData, roundResult);
                })
            .When(MessageTypes.AvailableCardsChanged)
                .Then( () => message = new Messages.Domain.WhiteCardPlayedMessage(messageData))
            .When(MessageTypes.TimerStarted)
            .Then(() => message = new Messages.Domain.TimerMessage(messageData))
            .DefaultCondition(() => throw new ArgumentException($"Invalid message type {messageData.MessageType}", nameof(messageData)));

        return message;
    }
    #endregion
}
