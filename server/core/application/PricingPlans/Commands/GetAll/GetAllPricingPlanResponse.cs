using LocadoraDeAutomoveis.Application.PricingPlans.Commands.Create;
using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.PricingPlans.Commands.GetAll;

public record GetAllPricingPlanResponse(
    int Quantity,
    ImmutableList<PricingPlanDto> PricingPlans
);

public record PricingPlanDto(
    Guid Id,
    Guid GroupId,
    DailyPlanDto DailyPlan,
    ControlledPlanDto ControlledPlan,
    FreePlanDto FreePlan
);