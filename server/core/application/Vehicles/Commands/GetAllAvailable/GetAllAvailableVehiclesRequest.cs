using FluentResults;
using LocadoraDeAutomoveis.Domain.Vehicles;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Vehicles.Commands.GetAllAvailable;

public record GetAllAvailableVehiclesRequest(
    int PageNumber = 1,
    int PageSize = 10,
    string? Term = null,
    Guid? GroupId = null,
    EFuelType? FuelType = null
) : IRequest<Result<GetAllAvailableVehiclesResponse>>;
