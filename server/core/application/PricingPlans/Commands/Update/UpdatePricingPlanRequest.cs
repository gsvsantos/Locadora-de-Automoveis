using FluentResults;
using LocadoraDeAutomoveis.Application.PricingPlans.Commands.Create;
using MediatR;

namespace LocadoraDeAutomoveis.Application.PricingPlans.Commands.Update;

public record UpdatePricingPlanRequestPartial(
    Guid GroupId,
    DailyPlanDto DailyPlan,
    ControlledPlanDto ControlledPlan,
    FreePlanDto FreePlan
) : IRequest<Result<UpdatePricingPlanResponse>>;

public record UpdatePricingPlanRequest(
    Guid Id,
    Guid GroupId,
    DailyPlanDto DailyPlan,
    ControlledPlanDto ControlledPlan,
    FreePlanDto FreePlan
) : IRequest<Result<UpdatePricingPlanResponse>>;
