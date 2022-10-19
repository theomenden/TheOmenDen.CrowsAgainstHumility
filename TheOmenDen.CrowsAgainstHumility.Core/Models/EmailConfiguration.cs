using System.ComponentModel.DataAnnotations;
#nullable disable
namespace TheOmenDen.CrowsAgainstHumility.Core.Models;
public sealed class EmailConfiguration
{
    public Int32 Id { get; set; }

    [MaxLength(50)]
    public String AnalyticsCode { get; set; }

    [MaxLength(50)]
    public String EmailAddress { get; set; }

    [MaxLength]
    public String EmailSenderName { get; set; }

    [MaxLength(255)]
    public String SendGridKey { get; set; }

    public String RegistrationApprovalMessage { get; set; }
}
