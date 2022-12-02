using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Services.Rooms;
using TheOmenDen.Shared.Extensions;

namespace TheOmenDen.CrowsAgainstHumility.Services.CardPoolBuilding;
public static class BlackCardProvider
{
    private static readonly object SyncObject = new ();

    private static readonly Dictionary<String, List<BlackCard>> BlackCardToRoomDictionary = new(StringComparer.OrdinalIgnoreCase);

    public static void AddRoomReference(CrowGameRoom room, IEnumerable<BlackCard> blackCards)
    {
        var blackCardList = blackCards.ToList();

        if (!BlackCardToRoomDictionary.ContainsKey(room.RoomName))
        {
            BlackCardToRoomDictionary.Add(room.RoomName, blackCardList);
        }

        BlackCardToRoomDictionary[room.RoomName] = blackCardList;
    }
    
    public static BlackCard GetBlackCardForRound(String roomName)
    {
        if (!BlackCardToRoomDictionary.TryGetValue(roomName, out var blackCardPool))
        {
            throw new ArgumentOutOfRangeException(nameof(roomName));
        }

        var blackCardToPlay = blackCardPool.GetRandomElement();
        
        lock (SyncObject)
        {
            RemoveUsedCardFromPool(roomName, blackCardToPlay);
        }

        return blackCardToPlay;
    }
    
    private static void RemoveUsedCardFromPool(String roomName, BlackCard blackCardToRemove) =>
        _ = BlackCardToRoomDictionary.TryGetValue(roomName, out var blackCards)
        && blackCards.Remove(blackCardToRemove);
}
