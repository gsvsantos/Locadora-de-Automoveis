using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Vehicles.Commands.GetAvailableById;

public record GetAvailableByIdVehicleRequest(
    Guid Id
) : IRequest<Result<GetAvailableByIdVehicleResponse>>;
