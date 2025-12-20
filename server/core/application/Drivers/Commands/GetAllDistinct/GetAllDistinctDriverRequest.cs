using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Drivers.Commands.GetAllDistinct;

public record GetAllDistinctDriverRequest(
    Guid VehicleId
) : IRequest<Result<GetAllDistinctDriverResponse>>;
