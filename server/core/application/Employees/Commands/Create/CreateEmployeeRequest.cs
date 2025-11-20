using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Employees.Commands.Create;

public record CreateEmployeeRequest(
    string FullName,
    DateTimeOffset AdmissionDate,
    decimal Salary,
    string UserName,
    string Email,
    string PhoneNumber,
    string Password
) : IRequest<Result<CreateEmployeeResponse>>;