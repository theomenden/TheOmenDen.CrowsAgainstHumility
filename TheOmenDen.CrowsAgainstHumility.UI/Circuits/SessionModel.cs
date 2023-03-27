namespace TheOmenDen.CrowsAgainstHumility.Circuits;
/// <summary>
/// Container for relevant session information
/// </summary>
public sealed class SessionModel : IEquatable<SessionModel>, IComparable<SessionModel>
{
    public SessionModel()
    {
        Id = Guid.NewGuid();
        Name = String.Empty;
        User = SessionUser.DefaultUser;
    }

    public SessionModel(String name)
    {
        Id = Guid.NewGuid();
        Name = name;
        User = SessionUser.DefaultUser;
    }

    public SessionModel(String name, SessionUser user)
    {
        Id = Guid.NewGuid();
        Name = name;
        User = user;
    }

    /// <summary>
    /// The unique Id for this session
    /// </summary>
    public Guid Id { get; }

    public SessionUser User { get; set; }

    /// <summary>
    /// The name associated with the session
    /// </summary>
    public String Name { get; }

    /// <summary>
    /// The id of the circuit associated with this session
    /// </summary>
    public String CircuitId { get; set; } = String.Empty;

    /// <summary>
    /// The time that the session was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    public int CompareTo(SessionModel? other) => CreatedAt.CompareTo(other?.CreatedAt);

    public bool Equals(SessionModel? other) =>
        other is not null &&
            Id == other.Id;
    
    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        return obj is SessionModel session && Equals(session);
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(SessionModel left, SessionModel right) => left is null ? right is null : left.Equals(right);

    public static bool operator !=(SessionModel left, SessionModel right) => !(left == right);

    public static bool operator <(SessionModel left, SessionModel right) => left.CompareTo(right) < 0;

    public static bool operator >(SessionModel left, SessionModel right) => left.CompareTo(right) > 0;

    public static bool operator <=(SessionModel left, SessionModel right) => left.CompareTo(right) <= 0;

    public static bool operator >=(SessionModel left, SessionModel right)=> left.CompareTo(right) >= 0;

    public readonly static SessionModel DefaultSession = new("Default");
}
