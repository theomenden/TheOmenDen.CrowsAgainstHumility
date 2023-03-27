using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.EventArgs;
using TheOmenDen.CrowsAgainstHumility.Services;

namespace TheOmenDen.CrowsAgainstHumility.Store;
public class CrowGameState
{
    #region Event Handlers
    public event EventHandler? RoundStarted;
    public event EventHandler? CardCzarChoiceStarted;
    public event EventHandler<PlayerWhiteCardChoiceEventArgs>? PlayerCardChoiceStarted;
    public event EventHandler? WhiteCardChosen;
    public event EventHandler<(List<PlayerScore> scores, WhiteCard playedWhiteCard, Int32 timeOut)>? TurnScoring;
    public event EventHandler<IWhiteCardPlayedEventArgs>? WhiteCardEventReceived;
    public event EventHandler<GameMessage>? ChatMessageReceived;
    #endregion
    private readonly List<GameMessage> _chatLog = Enumerable.Empty<GameMessage>().ToList();

    internal CrowGameState()
    {
    }
    #region Exposed Properties
    public IEnumerable<GameMessage> ChatLog => _chatLog;
    public Int32 CurrentRound { get; private set; } = 0;
    public Int32 RoundCount { get; private set; } = 0;

    public TurnTimer TurnTimer { get; } = new();

    public BlackCard? BlackCard { get; private set; } = null;

    public WhiteCard? WhiteCard { get; private set; } = null;
    #endregion
    #region Game State

    internal void NewRoundStarted(Int32 currentRound, Int32 roundCount, GameMessage? chatMessage)
    {
        CurrentRound= currentRound;
        RoundCount= roundCount;
        RoundStarted?.Invoke(this, EventArgs.Empty);

        if (chatMessage is not null)
        {
            AddChatMessage(chatMessage);
        }
    }

    internal void CardCzarChoiceStart(BlackCard blackCard, Int32 time)
    {
        BlackCard= blackCard;
        TurnTimer.StartTimer(time);
        CardCzarChoiceStarted?.Invoke(this, EventArgs.Empty);
    }

    internal void PlayerWhiteCardChoiceStart(PlayerDto player, WhiteCard whiteCard, Int32 time)
    {
        TurnTimer.StartTimer(time);
        PlayerCardChoiceStarted?.Invoke(this, new PlayerWhiteCardChoiceEventArgs(player, time));
    }

    internal void ChosenWhiteCard(WhiteCard chosenCard)
    {
        WhiteCard = chosenCard;
        WhiteCardChosen?.Invoke(this, EventArgs.Empty);
    }

    internal void TurnScoresReceived(List<PlayerScore> scores, WhiteCard playedWhiteCard, Int32 timeout)
    {
        WhiteCard = playedWhiteCard;
        TurnTimer.StopTimer();
        TurnScoring?.Invoke(this, (scores, playedWhiteCard, timeout));
    }
    #endregion
    #region Chat Messages
    internal void AddChatMessage(GameMessage chatMessage)
    {
        if (_chatLog.Count >= 75)
        {
            _chatLog.RemoveRange(0, _chatLog.Count - 74);
        }
        _chatLog.Add(chatMessage);
        ChatMessageReceived?.Invoke(this, chatMessage);
    }
    #endregion
}