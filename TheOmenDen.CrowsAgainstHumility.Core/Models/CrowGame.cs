using System.Collections.Concurrent;
using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Cards;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models;

public sealed record CrowGame(Guid CzarId,
    BlackCard CurrentBlackCard, 
    ConcurrentDictionary<Guid, CrowGamePlayer> ConnectedPlayers,
    DateTimeOffset TurnStartedAt,
    TimeSpan TurnLength);