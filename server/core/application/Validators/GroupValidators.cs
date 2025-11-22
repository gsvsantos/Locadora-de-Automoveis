using FluentValidation;
using LocadoraDeAutomoveis.Domain.Groups;

namespace LocadoraDeAutomoveis.Application.Validators;

public class GroupValidators : AbstractValidator<Group>
{
    public GroupValidators()
    {
        RuleFor(g => g.Name)
            .NotEmpty().WithMessage("The {PropertyName} cannot be empty.")
            .MaximumLength(255).WithMessage("The group name cannot exceed {MaxLength} characters.");
    }
}
