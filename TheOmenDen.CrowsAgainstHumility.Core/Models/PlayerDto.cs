namespace TheOmenDen.CrowsAgainstHumility.Core.Models;
public sealed class PlayerDto: IEquatable<PlayerDto>
{
    public PlayerDto(string name, Guid id, bool isConnected)
    {
        Name = name;
        Id = id;
        IsConnected = isConnected;
    }

    public Guid Id { get; set; }

    public string Name { get; set; }
    
    public bool IsConnected { get; set; }

    public bool Equals(PlayerDto? other) => other is not null 
                                            && (ReferenceEquals(this, other) 
                                                || Id.Equals(other.Id));

    public override bool Equals(object? obj) => obj is not null 
               && ( ReferenceEquals(this, obj) 
                    || obj is PlayerDto other 
                    && Equals(other));

    public override int GetHashCode()=> Id.GetHashCode();

    public static bool operator ==(PlayerDto? left, PlayerDto? right) => Equals(left, right);

    public static bool operator !=(PlayerDto? left, PlayerDto? right) => !Equals(left, right);
}
