using FluentResults;

namespace LocadoraDeAutomoveis.Application.PricingPlans;

public abstract class PricingPlanErrorResults
{
    public static Error GroupAlreadyHavePricingPlanError(string group)
    {
        return new Error("Group already has a pricing plan")
            .CausedBy($"The group '{group}' already has a pricing plan associated")
            .WithMetadata("ErrorType", "BadRequest");
    }
}
