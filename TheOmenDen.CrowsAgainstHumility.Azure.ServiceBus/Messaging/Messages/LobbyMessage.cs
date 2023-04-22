using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Messaging.Messages;
public class LobbyMessage: ILobbyMessage
{
    public LobbyMessage() {}

    public LobbyMessage(string lobbyCode, MessageTypes messageType)
    {
        ArgumentException.ThrowIfNullOrEmpty(lobbyCode);
        
        LobbyCode = lobbyCode;
        MessageType = messageType;
    }

    public string LobbyCode { get; set; } = String.Empty;
    public MessageTypes MessageType { get; set;}
}
