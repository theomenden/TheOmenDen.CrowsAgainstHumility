namespace TheOmenDen.CrowsAgainstHumility.Services;

public sealed class CrowGamePlayer : IComparable<CrowGamePlayer>, IEquatable<CrowGamePlayer>
{
    #region Fields
    private int _awesomePoints;
    private int? _displayPosition = null;
    #endregion
    #region Event Handlers
    public event EventHandler<int>? ScoreChanged;
    #endregion
    #region Properties
    public Guid Id { get; set; }
    public String Name { get; set; }
    public Boolean IsConnected { get; set; }

    public Int32 AwesomePoints
    {
        get => _awesomePoints;
        set
        {
            if (value == _awesomePoints)
            {
                return;
            }

            _awesomePoints = value;
            ScoreChanged?.Invoke(this, _awesomePoints);
        }
    }

    public Int32? DisplayPosition
    {
        get => _displayPosition;
        set
        {
            if (value == _displayPosition)
            {
                return;
            }
            _displayPosition = value;
            ScoreChanged?.Invoke(this, _awesomePoints);
        }
    }
    #endregion
    #region Overrides
    public int CompareTo(CrowGamePlayer? other)
    {
        throw new NotImplementedException();
    }

    public bool Equals(CrowGamePlayer? other)
    {
        return other is not null
            && Id == other.Id;
    }

    public override Boolean Equals(Object? obj) =>
        obj is not null
        && (
            ReferenceEquals(this, obj)
            || (obj is PlayerDto playerDto && Id.Equals(playerDto.Id))
            || obj is CrowGamePlayer other && Equals(other)
            );


    public override int GetHashCode() => Id.GetHashCode();
    #endregion
    #region Operator Overloads
    public static bool operator ==(CrowGamePlayer? left, CrowGamePlayer? right) => Equals(left, right);

    public static bool operator !=(CrowGamePlayer? left, CrowGamePlayer? right) => !Equals(left, right);

    public static bool operator <(CrowGamePlayer? left, CrowGamePlayer? right) => left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(CrowGamePlayer? left, CrowGamePlayer? right) => left is null || left.CompareTo(right) <= 0;

    public static bool operator >(CrowGamePlayer? left, CrowGamePlayer? right) => left is { } && left.CompareTo(right) > 0;

    public static bool operator >=(CrowGamePlayer? left, CrowGamePlayer? right) => left is null ? right is null : left.CompareTo(right) >= 0;
    #endregion
}