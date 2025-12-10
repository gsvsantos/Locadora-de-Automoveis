using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Employees.Commands.SelfUpdate;

public record SelfUpdateEmployeeRequestPartial(
    string FullName
);

public record SelfUpdateEmployeeRequest(
    Guid Id,
    string FullName
) : IRequest<Result<SelfUpdateEmployeeResponse>>;
