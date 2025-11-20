using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Employees.Commands.Delete;

public record DeleteEmployeeRequest(
    Guid Id
) : IRequest<Result<DeleteEmployeeResponse>>;
