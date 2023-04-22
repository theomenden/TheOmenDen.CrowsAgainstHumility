using TheOmenDen.CrowsAgainstHumility.Core.Engine.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Game;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Models;

namespace TheOmenDen.CrowsAgainstHumility.Core.Engine.Managers;
internal class ServerManager
{
    public static Player ChangePlayerType(Player player, GameRoles newRole) => player with { Type = newRole };
    #region Internal Methods
    internal static Player AddOrUpdatePlayer(CrowGameServer server, Guid recoveryId, string playerConnectionid, string playerName, GameRoles type)
    {
        if (server.Players.Any(p => p.Value.RecoveryId == recoveryId))
        {
            RecoverPlayer(server, recoveryId, playerConnectionid);
            return AwakenPlayer(server, playerConnectionid);
        }

        var publicId = GeneratePublicId(server.Players);
        var player = new Player(playerConnectionid, recoveryId, publicId, playerName, type, PlayerMode.Awake);
        server.Players[playerConnectionid] = player;

        return player;
    }
    internal static void RemovePlayer(CrowGameServer server, string playerConnectionId)
    {
        var player = server.Players[playerConnectionId];
        server.Players.Remove(playerConnectionId);
        SessionManager.RemovePlayer(server.CurrentSession, player.PublicId);
    }
    internal static IEnumerable<CrowGameServer> SetPlayerToSleepOnAllConnectedServers(IEnumerable<CrowGameServer> servers, string playerConnectionId)
    {
        var serversWithPlayer = servers.Where(s => s.Players.ContainsKey(playerConnectionId)).ToArray();
        
        Array.ForEach(serversWithPlayer, crowGameServer =>  SetPlayerToSleep(crowGameServer, playerConnectionId));

        return serversWithPlayer;
    }
    internal static (bool wasRemoved, Player? removedPlayer) TryRemovePlayer(CrowGameServer server, int playerPublicId)
    {
        var player = server.Players
            .FirstOrDefault(kvp => kvp.Value.PublicId == playerPublicId)
            .Value;

        if (player is null)
        {
            return (false, null);
        }
        RemovePlayer(server, player.ConnectionId);
        return (true, player);

    }
    internal static Player SetPlayerToSleep(CrowGameServer server, string playerConnectionId) => SetPlayerMode(server, playerConnectionId, PlayerMode.Asleep);
    internal static Player AwakenPlayer(CrowGameServer server, string playerConnectionId) => SetPlayerMode(server, playerConnectionId, PlayerMode.Awake);
    internal static Player GetPlayer(CrowGameServer server, string playerConnectionId) => server.Players[playerConnectionId];
    #endregion
    #region Private Methods
    private static void RecoverPlayer(CrowGameServer server, Guid recoveryId, string newPlayerConnectionId)
    {
        var playerWithRecoveryId = server.Players.FirstOrDefault(p => p.Value?.RecoveryId == recoveryId).Value;

        if (playerWithRecoveryId is null)
        {
            return;
        }

        server.Players.Remove(playerWithRecoveryId.ConnectionId);

        var reconnectedPlayer = playerWithRecoveryId with
        {
            ConnectionId = newPlayerConnectionId
        };

        server.Players.Add(reconnectedPlayer.ConnectionId, reconnectedPlayer);

    }

    private static int GeneratePublicId(IDictionary<string, Player>? serverPlayers)
    {
        if (serverPlayers is null or { Count: <= 0 })
        {
            return 0;
        }
        var highestId = serverPlayers.Max(p => p.Value.PublicId);

        return ++highestId;
    }

    private static Player SetPlayerMode(CrowGameServer server, string playerConnectionId, PlayerMode mode)
    {
        var player = GetPlayer(server, playerConnectionId);

        return player with
        {
            Mode = mode
        };
    }
    #endregion
}
