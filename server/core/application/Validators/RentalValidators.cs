using FluentValidation;
using LocadoraDeAutomoveis.Domain.Rentals;

namespace LocadoraDeAutomoveis.Application.Validators;

public class RentalValidator : AbstractValidator<Rental>
{
    public RentalValidator()
    {
        RuleFor(r => r.ClientId)
            .NotEmpty().WithMessage("O Cliente é obrigatório.");

        RuleFor(r => r.DriverId)
            .NotEmpty().WithMessage("O Condutor é obrigatório.");

        RuleFor(r => r.VehicleId)
            .NotEmpty().WithMessage("O Veículo é obrigatório.");

        RuleFor(r => r.PricingPlanId)
            .NotEmpty().WithMessage("O Plano de Cobrança é obrigatório.");

        RuleFor(r => r.StartDate)
            .NotEmpty().WithMessage("A Data de Início é obrigatória.");

        RuleFor(r => r.ExpectedReturnDate)
            .NotEmpty().WithMessage("A Data Prevista de Devolução é obrigatória.")
            .GreaterThan(r => r.StartDate)
            .WithMessage("A Data Prevista de Devolução deve ser maior que a Data de Início.");

        RuleFor(r => r.StartKm)
            .GreaterThanOrEqualTo(0)
            .WithMessage("A Quilometragem Inicial não pode ser negativa.");

        When(r => r.SelectedPlanType == EPricingPlanType.Controlled, () =>
        {
            RuleFor(r => r.EstimatedKilometers)
                .NotNull().WithMessage("Para o Plano Controlado, é obrigatório informar a Quilometragem Estimada.")
                .GreaterThan(0).WithMessage("A Quilometragem Estimada deve ser maior que zero.");
        });
    }
}