using FluentValidation;
using LocadoraDeAutomoveis.Domain.Coupons;

namespace LocadoraDeAutomoveis.Application.Validators;

public class CouponValidator : AbstractValidator<Coupon>
{
    public CouponValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Code is required.")
            .MinimumLength(3);

        RuleFor(c => c.DiscountValue)
            .GreaterThan(0)
            .WithMessage("Discount value must be greater than zero.");

        RuleFor(c => c.ExpirationDate)
            .GreaterThan(DateTimeOffset.UtcNow)
            .WithMessage("Expiration date must be in the future.");
    }
}
