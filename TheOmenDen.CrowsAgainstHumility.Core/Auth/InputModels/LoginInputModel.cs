using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable
namespace TheOmenDen.CrowsAgainstHumility.Core.Auth.InputModels;
public sealed class LoginInputModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public String Email { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public String Password { get; set; }

    [Display(Name = "Remember Me")]
    public Boolean IsRemembered { get; set; }

    public String Token { get; set; }
}
