using FluentValidation;
using LocadoraDeAutomoveis.Domain.Configurations;

namespace LocadoraDeAutomoveis.Application.Validators;

public class ConfigurationValidators : AbstractValidator<Configuration>
{
    public ConfigurationValidators()
    {
        RuleFor(x => x.GasolinePrice)
            .GreaterThan(0).WithMessage("The {PropertyName} must be greater than 0.");

        RuleFor(x => x.GasPrice)
            .GreaterThan(0).WithMessage("The {PropertyName} must be greater than 0.");

        RuleFor(x => x.DieselPrice)
            .GreaterThan(0).WithMessage("The {PropertyName} must be greater than 0.");

        RuleFor(x => x.AlcoholPrice)
            .GreaterThan(0).WithMessage("The {PropertyName} must be greater than 0.");

    }
}
