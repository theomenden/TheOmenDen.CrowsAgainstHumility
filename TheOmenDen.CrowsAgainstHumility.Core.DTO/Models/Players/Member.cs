using System.Globalization;
using TheOmenDen.CrowsAgainstHumility.Core.DTO.Models.Cards;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.DTO.Models.Players;
public class Member : Observer
{
    #region Private Members
    private WhiteCardDto? _playedCard;
    #endregion
    #region Constructors
    public Member(Lobby lobby, string name)
        : base(lobby, name)
    {
        Role = GameRoles.Player;
    }

    internal Member(Lobby lobby, Serializations.MemberData memberData)
    : base(lobby, memberData)
    {
        _playedCard = memberData.PlayedCard;
    }
    #endregion
    #region Public Properties
    public WhiteCardDto? WhiteCard
    {
        get => _playedCard;
        set
        {
            if (value is null || _playedCard == value)
            {
                return;
            }

            if (value is not null && !Lobby.AvailableWhiteCards.Contains(value))
            {
                var exceptionMessage = String.Format(CultureInfo.CurrentCulture,
                    Resources.Resources.WhiteCardIsNotAvailable, value.Text);

                throw new ArgumentException(exceptionMessage, nameof(value));
            }

            _playedCard = value;

            if (Lobby.State == LobbyState.RoundInProgress)
            {
                Lobby.OnPlayerPlaysACard(this);
            }
        }
    }
    #endregion
    #region Public Methods

    public void StartTimer(TimeSpan duration)
    {
        if (duration <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(duration), duration, Resources.Resources.InvalidTimerDuration);
        }

        Lobby.StartTimer(duration);
    }

    public void CancelTimer() => Lobby.CancelTimer();
    #endregion

    #region Internal Methods
    internal void ResetWhiteCard() => _playedCard = null;

    protected internal override Serializations.MemberData GetData()
    {
        var result = base.GetData();
        result.GameRole = GameRoles.Player;
        result.PlayedCard = WhiteCard;
        return result;
    }
    #endregion
}
