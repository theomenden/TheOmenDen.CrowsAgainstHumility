using System.ComponentModel.DataAnnotations;
using TheOmenDen.CrowsAgainstHumility.Core.Constants;
#nullable disable
namespace TheOmenDen.CrowsAgainstHumility.Core.Models;
public sealed class RegisterInputModel
{
    [Required]
    [DataType(DataType.Text)]
    [Display(Name = "First Name")]
    public String FirstName { get; set; }

    [Required]
    [DataType(DataType.Text)]
    [Display(Name = "Last Name")]
    public String LastName { get; set; }

    [Required]
    [DataType(DataType.Text)]
    [RegularExpression(RegexPatterns.UserName, ErrorMessage = "Username may only contain letters, numbers, underscores(_) and hyphens (-)")]
    [MaxLength(15, ErrorMessage = "The username must be between 3 to 15 characters")]
    [MinLength(3, ErrorMessage = "The username must be between 3 to 15 characters")]
    [Display(Name ="Username")]
    public String Username { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public String Email { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and most {1} characters long", MinimumLength = 8)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public String Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name ="Confirm Password")]
    [Compare(nameof(Password), ErrorMessage ="The password and confirmation doesn't match!")]
    public string ConfirmPassword { get; set; }
}
