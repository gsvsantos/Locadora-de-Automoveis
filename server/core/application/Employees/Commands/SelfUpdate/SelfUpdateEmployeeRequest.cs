using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Employees.Commands.SelfUpdate;

public record SelfUpdateEmployeeRequestPartial(
    string FullName,
    DateTimeOffset AdmissionDate,
    decimal Salary
);

public record SelfUpdateEmployeeRequest(
    Guid Id,
    string FullName,
    DateTimeOffset AdmissionDate,
    decimal Salary
) : IRequest<Result<SelfUpdateEmployeeResponse>>;
