using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Employees.Commands.GetById;

public record GetByIdEmployeeRequest(Guid Id) : IRequest<Result<GetByIdEmployeeResponse>>;
