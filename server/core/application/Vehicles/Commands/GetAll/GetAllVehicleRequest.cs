using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Vehicles.Commands.GetAll;

public record GetAllVehicleRequest(
    int? Quantity
) : IRequest<Result<GetAllVehicleResponse>>;
