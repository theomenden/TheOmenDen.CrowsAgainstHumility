using System.Text.RegularExpressions;
using FluentValidation;
using TheOmenDen.CrowsAgainstHumility.Core.Constants;
using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Core.Validators;
public class RegisterInputModelValidator : AbstractValidator<RegisterInputModel>
{
    public RegisterInputModelValidator()
    {
        RuleFor(rim => rim.FirstName)
            .NotEmpty()
            .MaximumLength(30)
            .WithName("First Name");

        RuleFor(rim => rim.LastName)
            .NotEmpty()
            .MaximumLength(30)
            .WithName("Last Name");

        RuleFor(rim => rim.Email)
            .NotEmpty()
            .MaximumLength(60)
            .EmailAddress();
        
        RuleFor(rim => rim.Username)
            .NotEmpty()
            .Matches(RegexPatterns.Username,
                RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)
            .WithMessage("Username may only contain letters, numbers, underscores(_) and hyphens (-)")
            .MaximumLength(15)
            .WithMessage("The username must be between 3 to 15 characters")
            .MinimumLength(3)
            .WithMessage("The username must be between 3 to 15 characters")
            .WithName("Username");

        RuleFor(rim => rim.Password)
            .NotEmpty()
            .MaximumLength(18)
            .MinimumLength(8)
                .WithMessage("The password must be at least 8 and at most 18 characters long")
            .Matches(RegexPatterns.CapitalCheck, RegexOptions.Compiled| RegexOptions.IgnoreCase)
                .WithMessage("Your password must contain at least one uppercase letter.")
            .Matches(RegexPatterns.LowerCheck, RegexOptions.Compiled | RegexOptions.IgnoreCase)
                .WithMessage("Your password must contain at least one lowercase letter.")
            .Matches(RegexPatterns.NumericCheck, RegexOptions.Compiled | RegexOptions.IgnoreCase)
                .WithMessage("Your password must contain at least one number.")
            .Matches(RegexPatterns.SpecialCheck, RegexOptions.Compiled | RegexOptions.IgnoreCase)
                .WithMessage("Your password must contain at least one (!? *.).");

        RuleFor(rim => rim.ConfirmPassword)
            .NotEmpty()
            .Equal(rim => rim.Password, StringComparer.OrdinalIgnoreCase)
            .WithMessage("The password and confirmation must be the same");
    }
}
