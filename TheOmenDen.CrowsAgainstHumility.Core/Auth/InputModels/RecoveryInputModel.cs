using System.ComponentModel.DataAnnotations;
#nullable disable
namespace TheOmenDen.CrowsAgainstHumility.Core.Auth.InputModels;
public sealed class RecoveryInputModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public String Email { get; set; } = String.Empty;
}
