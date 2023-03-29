using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Providers;
using TheOmenDen.CrowsAgainstHumility.Core.Results;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;
public class PlayerList
{
    private readonly List<Player> _players = new();
    private readonly List<Observer> _observers = new();
    private readonly GuidProvider _guidProvider;
    private RoundResult? _roundResult;

    #region Constructors
    public PlayerList(string name,
            IEnumerable<WhiteCard>? availableWhiteCards = null,
            DateTimeProvider? dateTimeProvider = null,
            GuidProvider? guidProvider = null)
    {
        if (String.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        DateTimeProvider = dateTimeProvider ?? DateTimeProvider.Default;
        _guidProvider = guidProvider ?? GuidProvider.Default;
        Name = name;
        AvailableWhiteCards = availableWhiteCards;
    }
    #endregion
    #region Public Properties
    public string Name { get; private set; }
    public IEnumerable<Observer> Observers => _observers;
    public IEnumerable<Player> Players => _players;
   // public CardTsar? CardTsar => Players.OfType<CardTsar>().FirstOrDefault();
    public IEnumerable<WhiteCard> AvailableWhiteCards { get; private set; }
    public PlayerListState State { get; private set; }
    public RoundResult? RoundResult => State == PlayerListState.RoundFinished ? _roundResult : null;

    public IEnumerable<CrowGameRoundPlayerStatus> CrowGameRoundPlayers =>
        State == PlayerListState.RoundInProgress
            ? _roundResult!.Select(p => new CrowGameRoundPlayerStatus(p.Key.Name, p.Value is not null)).ToList()
            : Enumerable.Empty<CrowGameRoundPlayerStatus>();
    public DateTime? TimerEndTime { get; private set; }
    public DateTimeProvider DateTimeProvider { get; }

    #endregion
}
