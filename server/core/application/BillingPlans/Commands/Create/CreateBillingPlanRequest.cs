using FluentResults;
using LocadoraDeAutomoveis.Domain.BillingPlans;
using MediatR;

namespace LocadoraDeAutomoveis.Application.BillingPlans.Commands.Create;

public record CreateBillingPlanRequest(
    Guid GroupId,
    DailyBillingDto DailyBilling,
    ControlledBillingDto ControlledBilling,
    FreeBillingDto FreeBilling
) : IRequest<Result<CreateBillingPlanResponse>>;

public record DailyBillingDto(
    decimal DailyRate,
    decimal PricePerKm
);

public record ControlledBillingDto(
    decimal DailyRate,
    decimal PricePerKmExtrapolated
);

public record FreeBillingDto(
    decimal FixedRate
);

public static class RecordExtensions
{
    public static DailyBilling ToProps(this DailyBillingDto dto)
    {
        return new(dto.DailyRate, dto.PricePerKm);
    }

    public static ControlledBilling ToProps(this ControlledBillingDto dto)
    {
        return new(dto.DailyRate, dto.PricePerKmExtrapolated);
    }

    public static FreeBilling ToProps(this FreeBillingDto dto)
    {
        return new(dto.FixedRate);
    }
}