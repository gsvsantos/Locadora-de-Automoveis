using AutoMapper;
using LocadoraDeAutomoveis.Application.Vehicles.Commands.Create;
using LocadoraDeAutomoveis.Application.Vehicles.Commands.GetAll;
using LocadoraDeAutomoveis.Application.Vehicles.Commands.GetAvailableById;
using LocadoraDeAutomoveis.Application.Vehicles.Commands.GetById;
using LocadoraDeAutomoveis.Application.Vehicles.Commands.Update;
using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.Vehicles;
using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Mappings;

public class VehicleProfile : Profile
{
    public VehicleProfile()
    {
        // CONTROLLER
        // GetAll
        CreateMap<GetAllVehicleRequestPartial, GetAllVehicleRequest>()
            .ConvertUsing(src => new GetAllVehicleRequest(
                src.Quantity,
                src.GroupId,
                src.IsActive
            ));

        // Update
        CreateMap<(UpdateVehicleRequestPartial p, Guid id), UpdateVehicleRequest>()
            .ConvertUsing(src => new UpdateVehicleRequest(
                src.id,
                src.p.LicensePlate,
                src.p.Brand,
                src.p.Color,
                src.p.Model,
                src.p.FuelType,
                src.p.FuelTankCapacity,
                src.p.Kilometers,
                src.p.Year,
                src.p.Image,
                src.p.GroupId
            ));

        // DTOs
        CreateMap<Group, VehicleGroupDto>()
            .ConvertUsing(src => new VehicleGroupDto(
                src.Id,
                src.Name,
                src.IsActive
            ));

        CreateMap<Vehicle, VehicleDto>()
            .ConvertUsing((src, dest, ctx) => new VehicleDto(
                src.Id,
                src.LicensePlate,
                src.Brand,
                src.Color,
                src.Model,
                src.FuelType,
                src.FuelTankCapacity,
                src.Kilometers,
                src.Year,
                src.Image,
                ctx.Mapper.Map<VehicleGroupDto>(src.Group),
                src.IsActive
            ));

        // HANDLERS
        // Create
        CreateMap<(CreateVehicleRequest r, string img), Vehicle>()
            .ConvertUsing(src => new Vehicle(
                src.r.LicensePlate,
                src.r.Brand,
                src.r.Color,
                src.r.Model,
                src.r.FuelTankCapacity,
                src.r.Kilometers,
                src.r.Year,
                src.img
            ));

        // GetAll
        CreateMap<List<Vehicle>, GetAllVehicleResponse>()
            .ConvertUsing((src, dest, ctx) => new GetAllVehicleResponse(
                src.Count,
                src.Select(c => ctx.Mapper.Map<VehicleDto>(c)).ToImmutableList()
                    ?? ImmutableList<VehicleDto>.Empty
            ));

        // GetById
        CreateMap<Vehicle, GetByIdVehicleResponse>()
            .ConvertUsing((src, dest, ctx) => new GetByIdVehicleResponse(
               ctx.Mapper.Map<VehicleDto>(src)
            ));

        // Update
        CreateMap<(UpdateVehicleRequest r, string img), Vehicle>()
            .ConvertUsing(src => new Vehicle(
                src.r.LicensePlate,
                src.r.Brand,
                src.r.Color,
                src.r.Model,
                src.r.FuelTankCapacity,
                src.r.Kilometers,
                src.r.Year,
                src.img
            )
            { Id = src.r.Id });

        // GetAvailableByIdVehicle
        CreateMap<Vehicle, GetAvailableByIdVehicleResponse>()
            .ConvertUsing((src, dest, ctx) => new GetAvailableByIdVehicleResponse(
               ctx.Mapper.Map<VehicleDto>(src)
            ));
    }
}
