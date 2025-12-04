using FluentValidation;
using LocadoraDeAutomoveis.Domain.RentalExtras;

namespace LocadoraDeAutomoveis.Application.Validators;

public class RentalExtraValidators : AbstractValidator<RentalExtra>
{
    public RentalExtraValidators()
    {
        RuleFor(m => m.Name)
            .NotEmpty().WithMessage("The {PropertyName} field is required")
            .MinimumLength(3)
            .WithMessage("The {PropertyName} field must contain at least {MinLength} characters");

        RuleFor(m => m.Price)
            .NotEmpty().WithMessage("The {PropertyName} field is required")
            .GreaterThan(0).WithMessage("The {PropertyName} field must be greater than zero");

        RuleFor(m => m.Type)
            .IsInEnum()
            .WithMessage("The value '{PropertyValue}' is not a valid Extra Type.");
    }
}
