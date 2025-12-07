using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Drivers.Commands.GetAll;

public record GetAllDriverRequestPartial(
    int? Quantity,
    bool? IsActive
);

public record GetAllDriverRequest(
    int? Quantity,
    bool? IsActive
) : IRequest<Result<GetAllDriverResponse>>;
