namespace TheOmenDen.CrowsAgainstHumility.Core.Models;
public sealed class RoomStateDto
{
    public RoomStateDto() {}

    public RoomStateDto(string roomName, IEnumerable<PlayerDto> players, CrowRoomSettings roomSettings)
    {
        RoomName = roomName;
        Players = players;
        RoomSettings = roomSettings;
    }

    public RoomStateDto(string roomName, IEnumerable<PlayerDto> players, CrowRoomSettings roomSettings, bool isGameInProgress)
    {
        RoomName = roomName;
        Players = players;
        RoomSettings = roomSettings;
        IsGameInProgress = isGameInProgress;
    }

    public string RoomName { get; set; } = String.Empty;

    public IEnumerable<PlayerDto> Players { get; set; } = Enumerable.Empty<PlayerDto>();

    public CrowRoomSettings RoomSettings { get; set; } = new();

    public Boolean IsGameInProgress { get; set; }
}
