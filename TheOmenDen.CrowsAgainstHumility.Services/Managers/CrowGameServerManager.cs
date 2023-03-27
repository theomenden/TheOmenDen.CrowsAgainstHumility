using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;

namespace TheOmenDen.CrowsAgainstHumility.Services.Managers;
internal static class CrowGameServerManager
{
    #region Public Static Methods
    public static Player ChangePlayerRole(Player player, GameRoles newRole)
        => player with { GameRole = newRole };
    #endregion
    #region Internal Static Methods
    internal static Player AddOrUpdatePlayer(CrowGameServer gameServer, Guid recoveryId, string playerPrivateId,
        string playerName, GameRoles type)
    {
        if (gameServer.Players.Any(p => p.Value.RecoveryId == recoveryId))
        {
            RecoverPlayer(gameServer, recoveryId, playerPrivateId);
            return MakePlayerTsar(gameServer, playerPrivateId);
        }

        var publicId = GeneratePublicId(gameServer.Players);
        var player = new Player { };
        gameServer.Players[playerPrivateId] = player;

        return player;
    }

    internal static Player GetPlayer(CrowGameServer server, string playerPrivateId)
    {
        var player = server.Players[playerPrivateId];
        return player;
    }

    internal static void RemovePlayer(CrowGameServer server, string playerPrivateId)
    {
        var player = server.Players[playerPrivateId];
        server.Players.Remove(playerPrivateId);
        SessionManager.RemovePlayer(server.CurrentSession, player.PublicId);
    }

    internal static (bool couldBeRemoved, Player? removedPlayer) TryRemovePlayer(CrowGameServer server, int playerPublicId)
    {
        var player = server.Players.FirstOrDefault(kvp => kvp.Value.PublicId == playerPublicId)?.Value;

        if (player is null)
        {
            return (false, null);
        }

        RemovePlayer(server, player.Id);

        return (true, player);
    }

    internal static IEnumerable<CrowGameServer> SetPlayerToInactiveOnAllServers(IEnumerable<CrowGameServer> servers,
        string playerPrivateId)
    {
        var serversWithUser = servers.Where(s => s.Players.ContainsKey(playerPrivateId)).ToList();
        
        foreach (var server in serversWithUser)
        {
            InactivatePlayer(server, playerPrivateId);
        }

        return serversWithUser;
    }

    internal static Player InactivatePlayer(CrowGameServer server, string playerPrivateId)
        => SetPlayerMode(server, playerPrivateId, GameRoles.Player);

    internal static Player MakePlayerTsar(CrowGameServer server, string playerPrivateId)
        => SetPlayerMode(server, playerPrivateId, GameRoles.CardTsar);
    #endregion
    #region Private static Methods
    private static int GeneratePublicId(IDictionary<string, Player> serverPlayers)
    {
        if (serverPlayers?.Count is 0)
        {
            return 0;
        }

        var highestId = serverPlayers.Max(p => p.Value.PublicId);
        return ++highestId;
    }

    private static Player SetPlayerMode(CrowGameServer server, string playerPrivateId, GameRoles mode)
    {
        var player = GetPlayer(server, playerPrivateId);
        return player with
        {
            GameRole = mode
        };
    }
    #endregion


}
