using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.Shared.Guards;

namespace TheOmenDen.CrowsAgainstHumility.Azure.Messages;
public class PlayerListMessage
{
    public PlayerListMessage() { }

    public PlayerListMessage(string playerListName, MessageTypes messageType)
    {
        Guard.FromNullOrWhitespace(playerListName, nameof(playerListName));

        PlayerListName = playerListName;
        MessageType = messageType;
    }

    public string PlayerListName { get; set; } = String.Empty;
    public MessageTypes MessageType { get; set; }
}
