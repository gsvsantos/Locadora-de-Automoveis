using AutoMapper;
using LocadoraDeAutomoveis.Application.Vehicles.Commands.Create;
using LocadoraDeAutomoveis.Application.Vehicles.Commands.GetAll;
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
                src.p.Year,
                src.p.PhotoPath,
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
                src.Year,
                src.PhotoPath ?? "path not found",
                ctx.Mapper.Map<VehicleGroupDto>(src.Group),
                src.IsActive
            ));

        // HANDLERS
        // Create
        CreateMap<CreateVehicleRequest, Vehicle>()
            .ConvertUsing(src => new Vehicle(
                src.LicensePlate,
                src.Brand,
                src.Color,
                src.Model,
                src.FuelTankCapacity,
                src.Year,
                src.PhotoPath ?? "path not found"
            ));

        // GetALl
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
        CreateMap<UpdateVehicleRequest, Vehicle>()
            .ConvertUsing(src => new Vehicle(
                src.LicensePlate,
                src.Brand,
                src.Color,
                src.Model,
                src.FuelTankCapacity,
                src.Year,
                src.PhotoPath ?? "path not found"
            )
            { Id = src.Id });
    }
}
