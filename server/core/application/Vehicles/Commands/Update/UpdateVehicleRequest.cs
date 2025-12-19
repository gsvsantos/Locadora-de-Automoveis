using FluentResults;
using LocadoraDeAutomoveis.Domain.Vehicles;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace LocadoraDeAutomoveis.Application.Vehicles.Commands.Update;

public record UpdateVehicleRequestPartial(
    string LicensePlate,
    string Brand,
    string Color,
    string Model,
    EFuelType FuelType,
    int FuelTankCapacity,
    decimal Kilometers,
    int Year,
    IFormFile? Image,
    Guid GroupId
);

public record UpdateVehicleRequest(
    Guid Id,
    string LicensePlate,
    string Brand,
    string Color,
    string Model,
    EFuelType FuelType,
    int FuelTankCapacity,
    decimal Kilometers,
    int Year,
    IFormFile? Image,
    Guid GroupId
) : IRequest<Result<UpdateVehicleResponse>>;
