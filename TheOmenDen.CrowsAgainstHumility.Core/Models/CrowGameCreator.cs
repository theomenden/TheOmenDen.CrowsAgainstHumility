namespace TheOmenDen.CrowsAgainstHumility.Core.Models;

public sealed record CrowGameCreator(IEnumerable<Pack> Packs, Guid CreatorUserId, IEnumerable<String> PlayerNames, String GameName, String RoomCode);