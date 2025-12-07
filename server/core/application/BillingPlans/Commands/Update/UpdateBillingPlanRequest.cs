using FluentResults;
using LocadoraDeAutomoveis.Application.BillingPlans.Commands.Create;
using MediatR;

namespace LocadoraDeAutomoveis.Application.BillingPlans.Commands.Update;

public record UpdateBillingPlanRequestPartial(
    Guid GroupId,
    DailyBillingDto DailyBilling,
    ControlledBillingDto ControlledBilling,
    FreeBillingDto FreeBilling
);

public record UpdateBillingPlanRequest(
    Guid Id,
    Guid GroupId,
    DailyBillingDto DailyBilling,
    ControlledBillingDto ControlledBilling,
    FreeBillingDto FreeBilling
) : IRequest<Result<UpdateBillingPlanResponse>>;
