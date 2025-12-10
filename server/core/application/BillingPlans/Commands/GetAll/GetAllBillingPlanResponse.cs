using LocadoraDeAutomoveis.Application.BillingPlans.Commands.Create;
using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.BillingPlans.Commands.GetAll;

public record GetAllBillingPlanResponse(
    int Quantity,
    ImmutableList<BillingPlanDto> BillingPlans
);

public record BillingPlanDto(
    Guid Id,
    BillingPlanGroupDto Group,
    DailyBillingDto DailyBilling,
    ControlledBillingDto ControlledBilling,
    FreeBillingDto FreeBilling,
    bool IsActive
);

public record BillingPlanGroupDto(
    Guid Id,
    string Name
);