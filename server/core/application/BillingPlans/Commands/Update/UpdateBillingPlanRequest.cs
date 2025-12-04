using FluentResults;
using LocadoraDeAutomoveis.Application.BillingPlans.Commands.Create;
using MediatR;

namespace LocadoraDeAutomoveis.Application.BillingPlans.Commands.Update;

public record UpdateBillingPlanRequestPartial(
    Guid GroupId,
    DailyPlanDto DailyPlan,
    ControlledPlanDto ControlledPlan,
    FreePlanDto FreePlan
);

public record UpdateBillingPlanRequest(
    Guid Id,
    Guid GroupId,
    DailyPlanDto DailyPlan,
    ControlledPlanDto ControlledPlan,
    FreePlanDto FreePlan
) : IRequest<Result<UpdateBillingPlanResponse>>;
