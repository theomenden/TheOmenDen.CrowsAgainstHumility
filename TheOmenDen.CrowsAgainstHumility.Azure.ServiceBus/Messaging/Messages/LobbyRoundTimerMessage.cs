using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Messaging.Messages;
public sealed class LobbyRoundTimerMessage: LobbyMessage
{
    public LobbyRoundTimerMessage() {}
    public LobbyRoundTimerMessage(string lobbyName, MessageTypes messageType) :base(lobbyName, messageType) { }

    public DateTime EndingAt { get; set; }
}