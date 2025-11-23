using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Drivers.Commands.GetAll;

public record GetAllDriverRequest(
    int? Quantity
) : IRequest<Result<GetAllDriverResponse>>;
