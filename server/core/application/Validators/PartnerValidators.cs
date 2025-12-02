using FluentValidation;
using LocadoraDeAutomoveis.Domain.Partners;

namespace LocadoraDeAutomoveis.Application.Validators;

public class PartnerValidator : AbstractValidator<Partner>
{
    public PartnerValidator()
    {
        RuleFor(p => p.FullName)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters.");
    }
}
