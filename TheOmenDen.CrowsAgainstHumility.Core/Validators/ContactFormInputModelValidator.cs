using System.Runtime.InteropServices;
using FluentValidation;
using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Core.Validators;
public sealed class ContactFormInputModelValidator: AbstractValidator<ContactFormInputModel>
{
    public ContactFormInputModelValidator()
    {
        RuleFor(cfm => cfm.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Please provide a valid email address");

        RuleFor(cfm => cfm.FirstName)
            .NotEmpty();
        RuleFor(cfm => cfm.LastName)
            .NotEmpty();

        RuleFor(cfm => cfm.Subject)
            .NotEmpty()
                .WithMessage("Please provide a subject so we can better assist you");

        RuleFor(cfm => cfm.Body)
            .NotEmpty()
            .MinimumLength(50)
                .WithMessage("Please write us a short detailed message with your issue")
            .MaximumLength(1000)
                .WithMessage("Please write us a short detailed message with your issue");
    }
}
