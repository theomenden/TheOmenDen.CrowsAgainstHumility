using TheOmenDen.Shared.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
public sealed record NodeMessageTypes : EnumerationBaseFlag<NodeMessageTypes, byte>
{
    private NodeMessageTypes(string name, byte id) : base(name, id) { }

    public static readonly NodeMessageTypes PlayerListMessage = new(nameof(PlayerListMessage), 1);
    public static readonly NodeMessageTypes LobbyCreated = new(nameof(LobbyCreated), 2);
    public static readonly NodeMessageTypes RequestPlayerList = new(nameof(RequestPlayerList), 4);
    public static readonly NodeMessageTypes PlayerList = new(nameof(PlayerList), 8);
    public static readonly NodeMessageTypes RequestPlayers = new(nameof(RequestPlayers), 16);
    public static readonly NodeMessageTypes InitializePlayers = new(nameof(InitializePlayers), 32);
}
