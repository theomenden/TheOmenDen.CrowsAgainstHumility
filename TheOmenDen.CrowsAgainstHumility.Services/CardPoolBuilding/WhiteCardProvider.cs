
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Services.Rooms;
using TheOmenDen.Shared.Extensions;

namespace TheOmenDen.CrowsAgainstHumility.Services.CardPoolBuilding;
public static class WhiteCardProvider
{
    private static readonly Dictionary<String, IEnumerable<WhiteCard>> WhiteCardPoolByRoomDictionary = new(StringComparer.OrdinalIgnoreCase);
    private const int MaxHandSize = 10;

    public static void CreateCardPoolProviderForRoom(CrowGameRoom room, IEnumerable<WhiteCard> whiteCards)
    {
        if (WhiteCardPoolByRoomDictionary.ContainsKey(room.RoomName))
        {
            WhiteCardPoolByRoomDictionary.Add(room.RoomName, whiteCards);
        }
    }

    public static bool RemoveCardPoolFromRoom(CrowGameRoom room) => WhiteCardPoolByRoomDictionary.Remove(room.RoomName);

    public static void AddHandsToPlayers(CrowGameRoom room)
    {
        var cardPool = WhiteCardPoolByRoomDictionary[room.RoomName].ToArray();

        foreach (var player in room.Players)
        {
            player.Hand.AddRange(cardPool.GetRandomElements(10));
        }
    }

    public static void ReplenishPlayerHand(CrowGameRoom room)
    {
        var cardPool = WhiteCardPoolByRoomDictionary[room.RoomName].ToArray();

        foreach (var playerHand in room.Players
                     .Where(p => !p.IsCardCzar)
                     .Select(p => p.Hand))
        {
            if (playerHand.Count >= MaxHandSize)
            {
                continue;
            }

            var lessThanHandSize = MaxHandSize - playerHand.Count;

            playerHand.AddRange(cardPool.GetRandomElements(lessThanHandSize));
        }
    }
}
