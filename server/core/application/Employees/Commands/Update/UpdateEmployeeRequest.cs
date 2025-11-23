using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Employees.Commands.Update;

public record UpdateEmployeeRequestPartial(
    string FullName,
    DateTimeOffset AdmissionDate,
    decimal Salary
);

public record UpdateEmployeeRequest(
    Guid Id,
    string FullName,
    DateTimeOffset AdmissionDate,
    decimal Salary
) : IRequest<Result<UpdateEmployeeResponse>>;
