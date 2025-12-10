using FluentResults;

namespace LocadoraDeAutomoveis.Application.Coupons;

public abstract class CouponErrorResults
{
    public static Error DuplicateNameError(string name)
    {
        return new Error("Duplicate name")
            .CausedBy($"An coupon with the name '{name}' has already been registered")
            .WithMetadata("ErrorType", "BadRequest");
    }
}
