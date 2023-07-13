using FluentValidation;
using TheOmenDen.CrowsAgainstHumility.Core.DTO.Models;
using TheOmenDen.Shared.Extensions;

namespace TheOmenDen.CrowsAgainstHumility.Core.Validators;
public sealed class CrowGameModelValidator: AbstractValidator<CrowGameInputModel>
{
    public CrowGameModelValidator()
    {
        RuleFor(crowGameInput => crowGameInput.RoomName)
            .NotEmpty()
            .MaximumLength(40)
            .WithName("Lobby Name")
            .WithMessage("Cannot have an empty Lobby Name");

        RuleFor(crowGameInput => crowGameInput.DesiredPacks)
            .NotEmpty()
            .WithName("Packs")
            .WithMessage("You must choose at least one pack");

        RuleFor(crowGameInput => crowGameInput.RoundTimeLimit)
            .GreaterThanOrEqualTo(5)
            .LessThanOrEqualTo(20)
            .WithMessage("You must choose a timer between 5 minutes and 20 minutes, 15 minutes a round is the default");

        RuleFor(crowGameInput => crowGameInput.DesiredPlayers)
            .NotEmpty()
            .Must(c => c.Count().IsBetween(4,10))
            .WithMessage("You must have between 4 and 10 players to create a game");
    }
}
