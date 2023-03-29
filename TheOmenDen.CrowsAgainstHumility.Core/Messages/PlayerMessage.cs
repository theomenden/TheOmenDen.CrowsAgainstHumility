using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;
using TheOmenDen.CrowsAgainstHumility.Core.Serialization;

namespace TheOmenDen.CrowsAgainstHumility.Core.Messages;
public class PlayerMessage : Message
{
    #region Constructors
    public PlayerMessage(MessageTypes type, Observer member) : base(type)
        => Member = member;

    internal PlayerMessage(Serialization.MessageData messageData, Observer member)
        : base(messageData) => Member = member;
    #endregion
    public Observer Member { get; }

    protected internal override MessageData GetData()
    {
        var result = base.GetData();
        result.PlayerName = Member.Name;
        return result;
    }
}
