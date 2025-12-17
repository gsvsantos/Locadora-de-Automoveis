using FluentResults;
using LocadoraDeAutomoveis.Domain.Vehicles;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace LocadoraDeAutomoveis.Application.Vehicles.Commands.Create;

public record CreateVehicleRequest(
    string LicensePlate,
    string Brand,
    string Color,
    string Model,
    EFuelType FuelType,
    int FuelTankCapacity,
    int Year,
    IFormFile? Image,
    Guid GroupId
) : IRequest<Result<CreateVehicleResponse>>;
