using FluentValidation;
using LocadoraDeAutomoveis.Domain.Drivers;
using System.Text.RegularExpressions;

namespace LocadoraDeAutomoveis.Application.Validators;

public class DriverValidators : AbstractValidator<Driver>
{
    public DriverValidators()
    {
        RuleFor(d => d.FullName)
            .NotEmpty().WithMessage("The Full Name is required.")
            .Length(3, 200).WithMessage("The Full Name must be between 3 and 200 characters.");

        RuleFor(d => d.Email)
            .NotEmpty().WithMessage("The Email is required.")
            .EmailAddress().WithMessage("The provided Email is not valid.")
            .MaximumLength(254);

        RuleFor(d => d.PhoneNumber)
            .NotEmpty().WithMessage("The Phone Number is required.")
            .Matches(@"^\(?\d{2}\)?\s?\d{4,5}-?\d{4}$").WithMessage("The Phone Number must be in a valid format (e.g., (11) 99999-9999).");

        RuleFor(d => d.Document)
            .NotEmpty().WithMessage("The CPF (Individual Tax ID) is required.")
            .Must(IsValidCpf).WithMessage("The provided CPF is invalid.");

        RuleFor(d => d.LicenseNumber)
            .NotEmpty().WithMessage("The Driver's License Number (CNH) is required.")
            .Length(11).WithMessage("The Driver's License Number must contain exactly 11 digits.")
            .Matches(@"^\d+$").WithMessage("The Driver's License Number must contain only numbers.");

        RuleFor(d => d.LicenseValidity)
            .GreaterThan(DateTimeOffset.UtcNow.Date)
            .WithMessage("The Driver's License is expired. Cannot register a driver with an expired license.");
    }

    private bool IsValidCpf(string? cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
        {
            return false;
        }

        cpf = Regex.Replace(cpf, "[^0-9]", "");

        if (cpf.Length != 11)
        {
            return false;
        }

        return true;
    }
}