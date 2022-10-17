﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
namespace TheOmenDen.CrowsAgainstHumility.Core.Models;

public partial class BlackCard: IComparable<BlackCard>, IEquatable<BlackCard>
{
    public Guid Id { get; set; }
    public string Message { get; set; }
    public int PickAnswersCount { get; set; }
    public Guid PackId { get; set; }
    public virtual Pack Pack { get; set; }
    
    public int CompareTo(BlackCard other) => Id.CompareTo(other.Id);

    public override string ToString() => Message ?? String.Empty;

    public bool Equals(BlackCard other)
        => other is not null 
           && Id == other.Id 
           && PackId == other.PackId
           && Message.Equals(other.Message, StringComparison.OrdinalIgnoreCase);

    public override bool Equals(object obj)
    {
        if (obj is null)
        {
            return false;
        }
        
        if (ReferenceEquals(this, obj))
        {
            return true;
        }
        
        return obj is BlackCard other && Equals(other);
    }

    public override int GetHashCode() => HashCode.Combine(Id, Message, PackId);

    public static bool operator ==(BlackCard left, BlackCard right) => left is null ? right is null : left.Equals(right);

    public static bool operator !=(BlackCard left, BlackCard right) => !(left == right);
}