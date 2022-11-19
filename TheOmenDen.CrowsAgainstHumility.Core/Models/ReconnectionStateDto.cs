namespace TheOmenDen.CrowsAgainstHumility.Core.Models;
public sealed class ReconnectionStateDto
{
    public ReconnectionStateDto(Guid playerId, RoomStateDto? roomState)
    {
        PlayerId = playerId;
        RoomState = roomState;
    }

    public Guid PlayerId { get; set; }

    public RoomStateDto? RoomState { get; set; }
}
