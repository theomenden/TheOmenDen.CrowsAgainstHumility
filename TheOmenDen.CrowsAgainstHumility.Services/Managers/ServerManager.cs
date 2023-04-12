using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;

namespace TheOmenDen.CrowsAgainstHumility.Services.Managers;
internal static class ServerManager
{
    #region Internal Methods
    internal static Observer AddOrUpdatePlayer(CrowGameServer server, Guid recoveryId, string playerPrivateId, string playerName, GameRoles playerType)
    {
        Observer player;

        if (server.Players.Any(p => p.Value.SessionId == recoveryId))
        {
            RecoverPlayer(server, recoveryId, playerPrivateId);
            player = WakePlayer(server, playerPrivateId);

            return player;
        }

        var publicId = GeneratePublicId(server.Players);
        player = new Observer(playerPrivateId, recoveryId, publicId, playerName, playerType);

        server.Players[playerPrivateId] = player;

        return player;
    }

    internal static Observer GetPlayer(CrowGameServer server, string playerPrivateId)
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

    internal static IList<CrowGameServer> SetPlayerToSleepOnAllServers(IEnumerable<CrowGameServer> servers,
        string playerPrivateId)
    {
        var serversWithUser = servers.Where(s => s.Players.ContainsKey(playerPrivateId)).ToArray();

        Array.ForEach(serversWithUser, server =>
        {
            SleepPlayer(server, playerPrivateId);
        });

        return serversWithUser;
    }

    internal static (bool couldBeRemoved, Observer? removedPlayer) TryRemovePlayer(CrowGameServer server,
        int playerPublicId)
    {
        var player = server.Players.Where(kvp => kvp.Value.PublicId == playerPublicId)
            .Select(kvp => kvp.Value)
            .FirstOrDefault();

        if (player is null)
        {
            return (false, null);
        }

        RemovePlayer(server, player.Id);
        return (true, player);

    }

    internal static Observer SleepPlayer(CrowGameServer server, string playerPrivateId) => SetPlayerMode(server, playerPrivateId, PlayerMode.Sleep);

    internal static Observer WakePlayer(CrowGameServer server, string playerPrivateId) => SetPlayerMode(server, playerPrivateId, PlayerMode.Awake);
    #endregion
    #region Private Methods
    private static void RecoverPlayer(CrowGameServer server, Guid recoveryId, Guid newPlayerPrivateId)
    {
        var playerWithRecoveryId = server.Players.FirstOrDefault(p => p.Value?.RecoveryId == recoveryId).Value;

        if (playerWithRecoveryId is null)
        {
            return;
        }

        server.Players.Remove(playerWithRecoveryId.Id);
        playerWithRecoveryId.Id = newPlayerPrivateId;
        server.Players.Add(playerWithRecoveryId.Id, playerWithRecoveryId);
    }

    private static int GeneratePublicId(IDictionary<string, Observer> serverPlayers)
    {
        var isEmpty = serverPlayers?.Count is 0;

        if (isEmpty)
        {
            return 0;
        }

        var highestId = serverPlayers.Max(p => p.Value.PublicId);
        return ++highestId;
    }

    private static Observer SetPlayerMode(CrowGameServer server, string playerPrivateId, PlayerMode mode)
    {
        var player = GetPlayer(server, playerPrivateId);
        player.Mode = mode;
        return player;
    }
    #endregion
    #region Public Methods

    public static Observer ChangePlayerType(CrowGameServer server, Observer player, GameRoles newType)
    {
        Observer? resultingPlayer = null;

        newType
            .When(GameRoles.Observer)
                .Then(() => resultingPlayer = player as Observer)
            .When(GameRoles.Player)
                .Then(() => resultingPlayer = player as Member)
            .When(GameRoles.CardTsar)
                .Then(() => resultingPlayer = player as CardTsar)
            .DefaultCondition(() => throw new InvalidOperationException($"Cannot convert to {newType}"));

        resultingPlayer!.Role = newType;

        return resultingPlayer!;
    }
    #endregion
}
