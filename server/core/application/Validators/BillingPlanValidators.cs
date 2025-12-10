using FluentValidation;
using LocadoraDeAutomoveis.Domain.BillingPlans;

namespace LocadoraDeAutomoveis.Application.Validators;

public class BillingPlanValidators : AbstractValidator<BillingPlan>
{
    public BillingPlanValidators()
    {
        RuleFor(p => p.Daily)
            .NotNull().WithMessage("The Daily Plan configuration is required.")
            .SetValidator(new DailyPlanPropsValidator());

        RuleFor(p => p.Controlled)
            .NotNull().WithMessage("The Controlled Plan configuration is required.")
            .SetValidator(new ControlledPlanPropsValidator());

        RuleFor(p => p.Free)
            .NotNull().WithMessage("The Free Plan configuration is required.")
            .SetValidator(new FreePlanPropsValidator());
    }
}

public class DailyPlanPropsValidator : AbstractValidator<DailyBilling>
{
    public DailyPlanPropsValidator()
    {
        RuleFor(x => x.DailyRate)
            .GreaterThan(0).WithMessage("The daily rate for the Daily Plan must be greater than 0.");

        RuleFor(x => x.PricePerKm)
            .GreaterThan(0).WithMessage("The price per km for the Daily Plan must be greater than 0.");
    }
}

public class ControlledPlanPropsValidator : AbstractValidator<ControlledBilling>
{
    public ControlledPlanPropsValidator()
    {
        RuleFor(x => x.DailyRate)
            .GreaterThan(0).WithMessage("The daily rate for the Controlled Plan must be greater than 0.");

        RuleFor(x => x.PricePerKmExtrapolated)
            .GreaterThan(0).WithMessage("The price per extrapolated km for the Controlled Plan must be greater than 0.");
    }
}

public class FreePlanPropsValidator : AbstractValidator<FreeBilling>
{
    public FreePlanPropsValidator()
    {
        RuleFor(x => x.FixedRate)
            .GreaterThan(0).WithMessage("The fixed daily rate for the Free Plan must be greater than 0.");
    }
}