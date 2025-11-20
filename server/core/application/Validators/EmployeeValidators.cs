using FluentValidation;
using LocadoraDeAutomoveis.Domain.Employees;

public class EmployeeValidators : AbstractValidator<Employee>
{
    public EmployeeValidators()
    {
        RuleFor(m => m.FullName)
            .NotEmpty().WithMessage("The {PropertyName} field is required")
            .MinimumLength(3)
            .WithMessage("The {PropertyName} field must contain at least {MinLength} characters");

        RuleFor(m => m.Salary)
            .NotEmpty().WithMessage("The {PropertyName} field is required")
            .GreaterThan(0).WithMessage("The {PropertyName} field must be greater than zero");

        RuleFor(m => m.AdmissionDate)
            .NotEmpty().WithMessage("The {PropertyName} field is required")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("The {PropertyName} field cannot be a future date");
    }
}