namespace TheOmenDen.CrowsAgainstHumility.Core.Models;
public sealed class CrowRoomSettings
{
    public CrowRoomSettings() {}

    public CrowRoomSettings(int playTime, bool shouldShowPlayedCards, bool isPrivateRoom, string roomCode)
    {
        CardPlayTime= playTime;
        ShowPlayedCards= shouldShowPlayedCards;
        IsPrivateRoom = isPrivateRoom;
        RoomCode= roomCode;
    }

    public CrowRoomSettings(CrowRoomSettings settings)
    {
        if (settings is null)
        {
            return;
        }

        CardPlayTime = settings.CardPlayTime;
        RoomCode= settings.RoomCode;
        ShowPlayedCards= settings.ShowPlayedCards;
        IsPrivateRoom= settings.IsPrivateRoom;
    }

    public Int32 CardPlayTime { get; set; } = 90;

    public Boolean ShowPlayedCards { get; set; } = false;

    public Boolean IsPrivateRoom { get; set; } = true;

    public String RoomCode { get; set; } = String.Empty;

    public Int32 Rounds { get; set; } = 40;
}
