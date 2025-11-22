using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Employees.Commands.GetAll;

public record GetAllEmployeeRequest(
    int? Quantity
) : IRequest<Result<GetAllEmployeeResponse>>;
