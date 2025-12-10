using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Vehicles.Commands.Delete;

public record DeleteVehicleRequest(
    Guid Id
) : IRequest<Result<DeleteVehicleResponse>>;
