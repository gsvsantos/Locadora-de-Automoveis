using FluentResults;

namespace LocadoraDeAutomoveis.Application.BillingPlans;

public abstract class BillingPlanErrorResults
{
    public static Error GroupAlreadyHaveBillingPlanError(string group)
    {
        return new Error("Group already has a billing plan")
            .CausedBy($"The group '{group}' already has a billing plan associated")
            .WithMetadata("ErrorType", "BadRequest");
    }
}
