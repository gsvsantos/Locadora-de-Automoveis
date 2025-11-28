using FluentValidation;
using LocadoraDeAutomoveis.Domain.Vehicles;

namespace LocadoraDeAutomoveis.Application.Validators;

public class VehicleValidators : AbstractValidator<Vehicle>
{
    public VehicleValidators()
    {
        RuleFor(v => v.LicensePlate)
            .NotEmpty().WithMessage("The {PropertyName} cannot be empty.")
            .MaximumLength(20).WithMessage("The license plate cannot exceed {MaxLength} characters.");

        RuleFor(v => v.Brand)
            .NotEmpty().WithMessage("The {PropertyName} cannot be empty.")
            .MaximumLength(100).WithMessage("The brand cannot exceed {MaxLength} characters.");

        RuleFor(v => v.Color)
            .NotEmpty().WithMessage("The {PropertyName} cannot be empty.")
            .MaximumLength(50).WithMessage("The color cannot exceed {MaxLength} characters.");

        RuleFor(v => v.Model)
            .NotEmpty().WithMessage("The {PropertyName} cannot be empty.")
            .MaximumLength(100).WithMessage("The model cannot exceed {MaxLength} characters.");

        RuleFor(v => v.FuelType)
            .IsInEnum()
            .WithMessage("The value '{PropertyValue}' is not a valid Fuel Type.");

        RuleFor(v => v.FuelTankCapacity)
            .GreaterThan(0).WithMessage("The {PropertyName} must be greater than zero.");

        RuleFor(v => v.Year)
            .GreaterThan(1950).WithMessage("The {PropertyName} must be between 1950 and {DateTime.UtcNow.Year}.");
    }
}