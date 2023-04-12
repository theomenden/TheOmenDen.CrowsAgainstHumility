using System.Globalization;
using TheOmenDen.CrowsAgainstHumility.Core.DTO.Messages.Domain;
using TheOmenDen.CrowsAgainstHumility.Core.DTO.Models.Cards;
using TheOmenDen.CrowsAgainstHumility.Core.DTO.Results.Domain;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Providers;

namespace TheOmenDen.CrowsAgainstHumility.Core.DTO.Models.Players;
public class Lobby
{
    private readonly List<Member> _members = new(10);
    private readonly List<Observer> _observers = new(5);
    private readonly GuidProvider _guidProvider;
    private RoundResult? _roundResult;
    #region Constructors
    public Lobby(string name)
        : this(name, null, null, null, null)
    { }

    public Lobby(string name,
        IEnumerable<WhiteCardDto>? availableWhiteCards,
        IEnumerable<BlackCardDto>? availableBlackCards,
        DateTimeProvider? dateTimeProvider,
        GuidProvider? guidProvider)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(name);

        DateTimeProvider = dateTimeProvider ?? DateTimeProvider.Default;
        _guidProvider = guidProvider ?? GuidProvider.Default;
        Name = name;
        AvailableWhiteCards = availableWhiteCards ?? Enumerable.Empty<WhiteCardDto>();
        AvailableBlackCards = availableBlackCards ?? Enumerable.Empty<BlackCardDto>();
    }
    #endregion
    #region Properties
    public event EventHandler<MessageReceivedEventArgs> MessageReceived;
    public IEnumerable<Observer> Observers => _observers;
    public IEnumerable<Member> Members => _members;
    public CardTsar CardTsar => Members.OfType<CardTsar>().FirstOrDefault();
    public string Name { get; private set; }
    public IEnumerable<WhiteCardDto> AvailableWhiteCards { get; private set; }
    public IEnumerable<BlackCardDto> AvailableBlackCards { get; private set; }

    public IEnumerable<RoundParticipantStatus> RoundParticipants => State == LobbyState.RoundInProgress
                ? _roundResult!.Select(x => new RoundParticipantStatus(x.Key.Name, x.Value is not null)).ToList()
                : Enumerable.Empty<RoundParticipantStatus>();


    public LobbyState State { get; private set; }
    public RoundResult? RoundResult => State == LobbyState.RoundFinished ? _roundResult : null;
    public DateTime? TimerEndsAt { get; private set; }
    public DateTimeProvider DateTimeProvider { get; }
    #endregion
    #region Public Methods

    public CardTsar SetCardTsar(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        if (FindMemberOrObserver(name) is not null)
        {
            throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, Resources.Resources.PlayerAlreadyExists, name), nameof(name));
        }

        if (State == LobbyState.RoundInProgress && CardTsar is not null)
        {
            throw new InvalidOperationException(Resources.Resources.CardTsarAlreadyExists);
        }

        var cardTsar = new CardTsar(this, name);
        _members.Add(cardTsar);
        cardTsar.SessionId = _guidProvider.NewGuid();

        var recipients = UnionMembersAndObservers().Where(m => m != cardTsar);

        SendMessage(recipients, () => new MemberMessage(MessageTypes.PlayerJoined, cardTsar));

        return cardTsar;
    }

    public Observer Join(string name, bool asObserver)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        if (FindMemberOrObserver(name) is not null)
        {
            throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, Resources.Resources.PlayerAlreadyExists, name), nameof(name));
        }

        Observer result;

        if (asObserver)
        {
            var observer = new Observer(this, name);
            _observers.Add(observer);
            result = observer;
            result.SessionId = _guidProvider.NewGuid();
        }
        else
        {
            var member = new Member(this, name);
            _members.Add(member);
            result = member;
        }

        result.SessionId = _guidProvider.NewGuid();
        var recipients = UnionMembersAndObservers().Where(m => m != result);

        SendMessage(recipients, () => new MemberMessage(MessageTypes.PlayerJoined, result));

        return result;
    }

    public void Disconnect(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        Observer? disconnectedObserver = null;
        var observer = _observers.FirstOrDefault(o => MatchObserverName(o, name));

        if (observer is not null)
        {
            _observers.Remove(observer);
            disconnectedObserver = observer;

        }

        if (disconnectedObserver is null)
        {
            var member = _members.FirstOrDefault(o => MatchObserverName(o, name));
            if (member is not null && !member.IsDormant)
            {

                DisconnectMember(member);
                disconnectedObserver = member;

                if (State == LobbyState.RoundInProgress)
                {
                    UpdateRoundResult(null);
                }
            }
        }

        if (disconnectedObserver is null)
        {
            return;
        }

        var recipients = UnionMembersAndObservers();
        SendMessage(recipients, () => new MemberMessage(MessageTypes.PlayerDisconnected, disconnectedObserver));

        disconnectedObserver.SendMessage(new Message(MessageTypes.Empty));
    }

    public Observer? CreateSession(string memberName)
    {
        var result = FindMemberOrObserver(memberName);

        if (result is not null)
        {
            result.SessionId = _guidProvider.NewGuid();
        }

        return result;
    }

    public Observer? FindMemberOrObserver(string name)
    {
        var allObservers = UnionMembersAndObservers();
        return allObservers.FirstOrDefault(o => MatchObserverName(o, name));
    }

    public void DisconnectInactiveObservers(TimeSpan inactivityTime)
    {
        var lastActiveAt = DateTimeProvider.UtcNow - inactivityTime;

        bool IsObserverCurrentlyActive(Observer observer) =>
            observer.LastActiveAt < lastActiveAt && !observer.IsDormant;

        var inactiveObservers = Observers.Where(IsObserverCurrentlyActive).ToList();
        var inactiveMembers = Members.Where(IsObserverCurrentlyActive).ToList();

        if (inactiveObservers.Count is 0 && inactiveMembers.Count is 0)
        {
            return;
        }

        foreach (var observer in inactiveObservers)
        {
            _observers.Remove(observer);
        }

        foreach (var member in inactiveMembers)
        {
            DisconnectMember(member);
        }

        var recipients = UnionMembersAndObservers().ToList();

        foreach (var member in inactiveObservers.Union(inactiveMembers))
        {
            SendMessage(recipients, () => new MemberMessage(MessageTypes.PlayerDisconnected, member));
        }

        if (inactiveMembers.Count > 0 && State == LobbyState.RoundInProgress)
        {
            UpdateRoundResult(null);
        }
    }

    public virtual Serializations.LobbyData GetData()
    => new()
    {
        Name = Name,
        AvailableCards = AvailableWhiteCards.ToList(),
        State = State,
        TimerEndedAt = TimerEndsAt,
        Members = UnionMembersAndObservers().Select(m => m.GetData()).ToList(),
        RoundResult = _roundResult?.GetData()
    };
    #endregion
    #region Internal Methods
    internal void StartRound()
    {
        State = LobbyState.RoundInProgress;

        foreach (var member in Members)
        {
            member.ResetWhiteCard();
        }

        _roundResult = new RoundResult(Members);

        var recipients = UnionMembersAndObservers();

        SendMessage(recipients, () => new Message(MessageTypes.GameRoundStarted));
    }

    internal void CancelRound()
    {
        State = LobbyState.RoundCanceled;

        _roundResult = null;

        var recipients = UnionMembersAndObservers();
        SendMessage(recipients, () => new Message(MessageTypes.GameRoundCanceled));
    }

    internal void StartTimer(TimeSpan duration)
    {
        var timerEndsAt = DateTimeProvider.UtcNow + duration;
        TimerEndsAt = timerEndsAt;

        var recipients = UnionMembersAndObservers();

        SendMessage(recipients, () => new TimerMessage(MessageTypes.TimerStarted, timerEndsAt));
    }

    internal void CancelTimer()
    {
        TimerEndsAt = null;

        var recipients = UnionMembersAndObservers();
        SendMessage(recipients, () => new Message(MessageTypes.TimerCanceled));
    }

    internal void OnPlayerPlaysACard(Member member)
    {
        var recipients = UnionMembersAndObservers();
        SendMessage(recipients, () => new MemberMessage(MessageTypes.PlayerPlayedACard, member));
    }

    internal void OnObserverActivity(Observer observer) =>
        SendMessage(new MemberMessage(MessageTypes.PlayerActivity, observer));
    #endregion
    #region Protected Methods
    protected virtual void OnMessageReceived(MessageReceivedEventArgs e) => MessageReceived?.Invoke(this, e);
    #endregion
    #region Private Methods
    private static bool MatchObserverName(Observer observer, string name) =>
        String.Equals(observer.Name, name, StringComparison.OrdinalIgnoreCase);

    private IEnumerable<Observer> UnionMembersAndObservers()
    {
        foreach (var member in Members)
        {
            yield return member;
        }

        foreach (var observer in Observers)
        {
            yield return observer;
        }
    }

    private void SendMessage(Message message)
    {
        if (message is null)
        {
            return;
        }

        OnMessageReceived(new MessageReceivedEventArgs(message));
    }

    private void SendMessage(IEnumerable<Observer> recipients, Func<Message> messageFactory)
    {
        SendMessage(messageFactory());

        foreach (var recipient in recipients)
        {
            recipient.SendMessage(messageFactory());
        }
    }

    private void DisconnectMember(Member member)
    {
        if (member is CardTsar)
        {
            member.IsDormant = true;
            return;
        }

        _members.Remove(member);
    }

    private void UpdateRoundResult(Member? member)
    {
        if (_roundResult is null)
        {
            return;
        }

        if (member is not null
            && _roundResult.ContainsMember(member))
        {
            _roundResult[member] = member.WhiteCard;
        }

        if (!_roundResult.All(x => x.Value is not null || !Members.Contains(x.Key)))
        {
            return;
        }

        _roundResult.SetToReadOnly();
        State = LobbyState.RoundFinished;

        var recipients = UnionMembersAndObservers();

        SendMessage(recipients, () => new RoundResultMessage(MessageTypes.GameRoundEnded, _roundResult));
    }

    private void DeserializeMembers(Serializations.LobbyData lobbyData)
    {
        var hasDuplicates = lobbyData.Members.GroupBy(m => m.Name, StringComparer.OrdinalIgnoreCase)
            .Any(g => g.Count() > 1);

        if (hasDuplicates)
        {
            throw new ArgumentException("Lobby members must have unique names.", nameof(lobbyData));
        }

        var cardTsarData = lobbyData.Members.SingleOrDefault(m => m.GameRole == GameRoles.CardTsar);

        if (cardTsarData is not null)
        {
            _members.Add(new CardTsar(this, cardTsarData));
        }

        foreach (var memberData in lobbyData.Members.Where(m => m.GameRole == GameRoles.Player))
        {
            _members.Add(new Member(this, memberData));
        }

        foreach (var observerData in lobbyData.Members.Where(m => m.GameRole == GameRoles.Observer))
        {
            _observers.Add(new Member(this, observerData));
        }
    }

    private void DeserializeRoundResult(Serializations.LobbyData lobbyData)
    {
        if (lobbyData.RoundResult is null)
        {
            return;
        }

        _roundResult = new RoundResult(this, lobbyData.RoundResult);

        if (State == LobbyState.RoundFinished)
        {
            _roundResult.SetToReadOnly();
        }
    }
    #endregion
}
