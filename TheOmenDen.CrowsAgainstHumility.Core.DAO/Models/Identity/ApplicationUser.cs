using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Identity;
public class ApplicationUser : IdentityUser<Guid>
{
    [Required]
    public DateTime CreatedDate { get; set; }

    [Required]
    [MaxLength(256)]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(256)]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }

    [Required]
    [MaxLength(256)]
    [Display(Name = "User Name")]
    public override string UserName { get; set; }

    [Required]
    [MaxLength(256)]
    [EmailAddress]
    public override string Email { get; set; }
    
    public string? ImageUrl { get; set; }

    public string GetUIImageUrl => string.IsNullOrWhiteSpace(ImageUrl)
        ? @"img\avatars\avatar-00.png"
        : ImageUrl;
}