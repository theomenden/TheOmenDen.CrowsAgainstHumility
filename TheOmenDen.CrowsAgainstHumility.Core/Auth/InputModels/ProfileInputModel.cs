using System.ComponentModel.DataAnnotations;

namespace TheOmenDen.CrowsAgainstHumility.Core.Auth.InputModels;
public sealed class ProfileInputModel
{
    [MaxLength(30, ErrorMessage = "Your display name must not be more than 30 characters.")]
    [Display(Name = "Display Name")]
    public String DisplayName { get; set; }
}
