using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Employees.Commands.GetAll;

public record GetAllEmployeeRequestPartial(
    int? Quantity,
    bool? IsActive
) : IRequest<Result<GetAllEmployeeResponse>>;

public record GetAllEmployeeRequest(
    int? Quantity,
    bool? IsActive
) : IRequest<Result<GetAllEmployeeResponse>>;
