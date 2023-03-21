using System.ComponentModel.DataAnnotations;
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
    [Display(Name ="Username")]
    public String Username { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public String Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public String Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name ="Confirm Password")]
    public string ConfirmPassword { get; set; }
}
