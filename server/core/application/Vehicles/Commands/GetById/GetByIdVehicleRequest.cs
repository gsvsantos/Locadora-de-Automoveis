using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Vehicles.Commands.GetById;

public record GetByIdVehicleRequest(
    Guid Id
) : IRequest<Result<GetByIdVehicleResponse>>;
