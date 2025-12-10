using FluentValidation;
using LocadoraDeAutomoveis.Domain.Rentals;

namespace LocadoraDeAutomoveis.Application.Validators;

public class RentalValidator : AbstractValidator<Rental>
{
    public RentalValidator()
    {
        RuleFor(r => r.ClientId)
            .NotEmpty().WithMessage("The Client is required.");

        RuleFor(r => r.DriverId)
            .NotEmpty().WithMessage("The Driver is required.");

        RuleFor(r => r.VehicleId)
            .NotEmpty().WithMessage("The Vehicle is required.");

        RuleFor(r => r.BillingPlanId)
            .NotEmpty().WithMessage("The Billing Plan is required.");

        RuleFor(r => r.StartDate)
            .NotEmpty().WithMessage("The Start Date is required.");

        RuleFor(r => r.ExpectedReturnDate)
            .NotEmpty().WithMessage("The Expected Return Date is required.")
            .GreaterThan(r => r.StartDate)
            .WithMessage("The Expected Return Date must be greater than the Start Date.");

        RuleFor(r => r.StartKm)
            .GreaterThanOrEqualTo(0)
            .WithMessage("The Start Km cannot be negative.");

        When(r => r.BillingPlanType == EBillingPlanType.Controlled, () =>
        {
            RuleFor(r => r.EstimatedKilometers)
                .NotNull().WithMessage("For the Controlled Plan, the Estimated Kilometers is required.")
                .GreaterThan(0).WithMessage("The Estimated Kilometers must be greater than zero.");
        });
    }
}