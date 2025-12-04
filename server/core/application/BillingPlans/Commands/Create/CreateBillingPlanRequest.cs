using FluentResults;
using LocadoraDeAutomoveis.Domain.BillingPlans;
using MediatR;

namespace LocadoraDeAutomoveis.Application.BillingPlans.Commands.Create;

public record CreateBillingPlanRequest(
    Guid GroupId,
    DailyPlanDto DailyPlan,
    ControlledPlanDto ControlledPlan,
    FreePlanDto FreePlan
) : IRequest<Result<CreateBillingPlanResponse>>;

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
    public static DailyPlanProps ToProps(this DailyPlanDto dto)
    {
        return new(dto.DailyRate, dto.PricePerKm);
    }

    public static ControlledPlanProps ToProps(this ControlledPlanDto dto)
    {
        return new(dto.DailyRate, dto.PricePerKmExtrapolated);
    }

    public static FreePlanProps ToProps(this FreePlanDto dto)
    {
        return new(dto.FixedRate);
    }
}