using FluentValidation;
using LocadoraDeAutomoveis.Domain.Rentals;

namespace LocadoraDeAutomoveis.Application.Validators;

public class RentalReturnValidator : AbstractValidator<RentalReturn>
{
    public RentalReturnValidator()
    {
        RuleFor(r => r.ReturnDate)
            .NotEmpty().WithMessage("The Return Date is required.")
            .GreaterThan(r => r.Rental.StartDate)
            .WithMessage("The Return Date must be greater than the Start Date.");

        RuleFor(r => r.EndKm)
            .GreaterThan(0)
            .WithMessage("The final odometer reading must be greater than zero.");

        RuleFor(r => r.TotalMileage)
            .GreaterThan(0)
            .WithMessage("The total mileage must be greater than zero.");

        RuleFor(r => r.FuelLevelAtReturn)
            .IsInEnum()
            .WithMessage("The Fuel Level is invalid.");
    }
}
