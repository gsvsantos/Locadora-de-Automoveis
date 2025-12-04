using FluentResults;
using LocadoraDeAutomoveis.Domain.BillingPlans;
using MediatR;

namespace LocadoraDeAutomoveis.Application.BillingPlans.Commands.Create;

public record CreateBillingPlanRequest(
    Guid GroupId,
    DailyPlanDto DailyBilling,
    ControlledPlanDto ControlledBilling,
    FreePlanDto FreeBilling
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
    public static DailyBilling ToProps(this DailyPlanDto dto)
    {
        return new(dto.DailyRate, dto.PricePerKm);
    }

    public static ControlledBilling ToProps(this ControlledPlanDto dto)
    {
        return new(dto.DailyRate, dto.PricePerKmExtrapolated);
    }

    public static FreeBilling ToProps(this FreePlanDto dto)
    {
        return new(dto.FixedRate);
    }
}