using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Vehicles.Commands.GetAll;

public record GetAllVehicleRequestPartial(
    int? Quantity,
    Guid? GroupId,
    bool? IsActive
) : IRequest<Result<GetAllVehicleResponse>>;

public record GetAllVehicleRequest(
    int? Quantity,
    Guid? GroupId,
    bool? IsActive
) : IRequest<Result<GetAllVehicleResponse>>;
