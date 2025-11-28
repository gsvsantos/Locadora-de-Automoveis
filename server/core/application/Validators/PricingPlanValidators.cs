using FluentValidation;
using LocadoraDeAutomoveis.Domain.PricingPlans;

namespace LocadoraDeAutomoveis.Application.Validators;

public class PricingPlanValidators : AbstractValidator<PricingPlan>
{
    public PricingPlanValidators()
    {
        RuleFor(p => p.DailyPlan)
            .NotNull().WithMessage("The Daily Plan configuration is required.")
            .SetValidator(new DailyPlanPropsValidator());

        RuleFor(p => p.ControlledPlan)
            .NotNull().WithMessage("The Controlled Plan configuration is required.")
            .SetValidator(new ControlledPlanPropsValidator());

        RuleFor(p => p.FreePlan)
            .NotNull().WithMessage("The Free Plan configuration is required.")
            .SetValidator(new FreePlanPropsValidator());
    }
}

public class DailyPlanPropsValidator : AbstractValidator<DailyPlanProps>
{
    public DailyPlanPropsValidator()
    {
        RuleFor(x => x.DailyRate)
            .GreaterThan(0).WithMessage("The daily rate for the Daily Plan must be greater than 0.");

        RuleFor(x => x.PricePerKm)
            .GreaterThan(0).WithMessage("The price per km for the Daily Plan must be greater than 0.");
    }
}

public class ControlledPlanPropsValidator : AbstractValidator<ControlledPlanProps>
{
    public ControlledPlanPropsValidator()
    {
        RuleFor(x => x.DailyRate)
            .GreaterThan(0).WithMessage("The daily rate for the Controlled Plan must be greater than 0.");

        RuleFor(x => x.PricePerKmExtrapolated)
            .GreaterThan(0).WithMessage("The price per extrapolated km for the Controlled Plan must be greater than 0.");
    }
}

public class FreePlanPropsValidator : AbstractValidator<FreePlanProps>
{
    public FreePlanPropsValidator()
    {
        RuleFor(x => x.FixedRate)
            .GreaterThan(0).WithMessage("The fixed daily rate for the Free Plan must be greater than 0.");
    }
}