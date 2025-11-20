using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Employees.Commands.GetAll;

public record GetAllEmployeeRequest() : IRequest<Result<GetAllEmployeeResponse>>;
