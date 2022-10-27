using System.ComponentModel.DataAnnotations;
#nullable disable
namespace TheOmenDen.CrowsAgainstHumility.Core.Auth.InputModels;
public sealed class ResetPasswordInputModel
{
    [Required]
    [EmailAddress]
    [Display(Name ="Email")]
    public string Email { get; set; }
    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
        MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name ="Confirm Password")]
    [Compare(nameof(Password), ErrorMessage ="The password and confirmation password don't match")]
    public string ConfirmPassword { get; set; }

    public string Token { get; set; }
}
