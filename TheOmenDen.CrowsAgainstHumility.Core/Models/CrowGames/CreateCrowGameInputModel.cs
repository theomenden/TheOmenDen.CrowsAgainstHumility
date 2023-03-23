namespace TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;
public sealed class CreateCrowGameInputModel
{
    public Guid Id { get; set; }
    public string RoomCode { get; set; } = String.Empty;
    public string RoomName { get; set; } = String.Empty;
    public string? Password { get; set; }
    
}
