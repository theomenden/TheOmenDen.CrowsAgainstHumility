using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.DTO.Models.Players;
public sealed class CardTsar : Member
{
    #region Constructors
    public CardTsar(Lobby lobby, string name)
        : base(lobby, name)
    {
    }

    internal CardTsar(Lobby lobby, Serializations.MemberData memberData)
        : base(lobby, memberData)
    {
    }
    #endregion
    #region Public Methods
    public void StartRound()
    {
        if (Lobby.State == LobbyState.RoundInProgress)
        {
            throw new InvalidOperationException(Resources.Resources.RoundIsInProgress);
        }

        Lobby.StartRound();
    }

    public void CancelRound()
    {
        if (Lobby.State == LobbyState.RoundInProgress)
        {
            Lobby.CancelRound();
        }
    }
    #endregion
    #region Overrides
    protected internal override Serializations.MemberData GetData()
    {
        var result = base.GetData();
        result.GameRole = GameRoles.CardTsar;
        return result;
    }
    #endregion
}
