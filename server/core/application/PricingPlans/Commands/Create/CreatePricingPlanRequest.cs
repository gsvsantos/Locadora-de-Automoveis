using FluentResults;
using LocadoraDeAutomoveis.Domain.PricingPlans;
using MediatR;

namespace LocadoraDeAutomoveis.Application.PricingPlans.Commands.Create;

public record CreatePricingPlanRequest(
    Guid GroupId,
    DailyPlanDto DailyPlan,
    ControlledPlanDto ControlledPlan,
    FreePlanDto FreePlan
) : IRequest<Result<CreatePricingPlanResponse>>;

public record DailyPlanDto(
    decimal DailyRate,
    decimal PricePerKm
);

public record ControlledPlanDto(
    decimal DailyRate,
    decimal PricePerKmExtrapolated
);

public record FreePlanDto(
    decimal FixedRate
);

public static class RecordExtensions
{
    public static DailyPlanProps ToProps(this DailyPlanDto dto) =>
        new(dto.DailyRate, dto.PricePerKm);
    public static ControlledPlanProps ToProps(this ControlledPlanDto dto) =>
        new(dto.DailyRate, dto.PricePerKmExtrapolated);
    public static FreePlanProps ToProps(this FreePlanDto dto) => new(dto.FixedRate);

}