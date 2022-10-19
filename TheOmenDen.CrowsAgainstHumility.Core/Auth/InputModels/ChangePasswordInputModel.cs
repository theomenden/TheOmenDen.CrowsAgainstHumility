using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheOmenDen.CrowsAgainstHumility.Core.Auth.InputModels;
public sealed class ChangePasswordInputModel
{
    [Required]
    [DataType(DataType.Password)]
    [Display(Name ="Current Password")]
    public String CurrentPassword { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display( Name = "New Password")]
    public String NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Display( Name = "Confirm new password")]
    [Compare(nameof(NewPassword), ErrorMessage ="The new password and confirmation don't match.")]
    public String ConfirmPassword { get; set; }
}
