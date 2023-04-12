using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Azure.Messages;

public class PlayerListTimerMessage :  PlayerListMessage
{
    public PlayerListTimerMessage() {}
    public PlayerListTimerMessage(string playerListName, MessageTypes messageType) :base(playerListName, messageType) { }

    public DateTime EndTime { get; set; }
}
