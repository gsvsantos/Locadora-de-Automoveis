using FluentValidation;
using LocadoraDeAutomoveis.Domain.Clients;
using System.Text.RegularExpressions;

namespace LocadoraDeAutomoveis.Application.Validators;

public class ClientValidators : AbstractValidator<Client>
{
    public ClientValidators()
    {
        RuleFor(c => c.FullName)
            .NotEmpty().WithMessage("The Full Name is required.")
            .Length(3, 200).WithMessage("The Full Name must be between 3 and 200 characters.");

        RuleFor(c => c.Email)
            .NotEmpty().WithMessage("The Email is required.")
            .EmailAddress().WithMessage("The provided Email is not valid.")
            .MaximumLength(254);

        RuleFor(c => c.PhoneNumber)
            .NotEmpty().WithMessage("The Phone Number is required.")
            .Matches(@"^\(?\d{2}\)?\s?\d{4,5}-?\d{4}$").WithMessage("The Phone Number must be in a valid format (e.g., (11) 99999-9999).");

        RuleSet("Complete", () =>
        {
            RuleFor(c => c.Document)
                .NotEmpty().WithMessage("Document is required to complete the profile.");

            When(c => !string.IsNullOrEmpty(c.Document) && c.Type == EClientType.Business, () =>
            {
                RuleFor(c => c.Document)
                    .Must(IsValidCnpj).WithMessage("The provided Business Document (CNPJ) is invalid.");
            });

            When(c => !string.IsNullOrEmpty(c.Document) && c.Type == EClientType.Individual, () =>
            {
                RuleFor(c => c.Document)
                    .Must(IsValidCpf).WithMessage("The provided Individual Document (CPF) is invalid.");
            });

            RuleFor(c => c.Address)
                .NotNull().WithMessage("Address is required to complete the profile.");

            When(c => c.Address != null, () =>
            {
                RuleFor(c => c.Address!.State)
                    .NotEmpty().WithMessage("The State is required.")
                    .Length(2).WithMessage("The State must be 2 letters.");

                RuleFor(c => c.Address!.City)
                    .NotEmpty().WithMessage("The City is required.");

                RuleFor(c => c.Address!.Neighborhood)
                    .NotEmpty().WithMessage("The Neighborhood is required.");

                RuleFor(c => c.Address!.Street)
                    .NotEmpty().WithMessage("The Street is required.");

                RuleFor(c => c.Address!.Number)
                    .GreaterThan(0).WithMessage("The Number must be greater than zero.");
            });
        });
    }

    private bool IsValidCpf(string? cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
        {
            return false;
        }

        cpf = Regex.Replace(cpf, "[^0-9]", "");
        return cpf.Length == 11;
    }

    private bool IsValidCnpj(string? cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
        {
            return false;
        }

        cnpj = Regex.Replace(cnpj, "[^0-9]", "");
        return cnpj.Length == 14;
    }
}