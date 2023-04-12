using TheOmenDen.CrowsAgainstHumility.Core.DTO.Models.Players;
using TheOmenDen.CrowsAgainstHumility.Core.DTO.Serializations;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.DTO.Messages.Domain;
public sealed class MemberMessage : Message
{
    #region Constructors
    public MemberMessage(MessageTypes type, Observer member)
    : base(type)
    {
        Member = member;
    }

    internal MemberMessage(Serializations.MessageData messageData, Observer member)
        : base(messageData)
    {
        Member = member;
    }
    #endregion
    #region Properties
    public Observer Member { get; }
    #endregion
    #region Overrides
    protected internal override MessageData GetData()
    {
        var result = base.GetData();
        result.MemberName = Member.Name;
        return result;
    }

    #endregion
}
