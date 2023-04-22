using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Messaging.Messages;
public interface ILobbyMessage
{
    string LobbyCode { get; }
    MessageTypes MessageType { get; }
}
