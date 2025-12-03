using AutoMapper;
using LocadoraDeAutomoveis.Application.Rentals.Commands.Create;
using LocadoraDeAutomoveis.Application.Rentals.Commands.GetAll;
using LocadoraDeAutomoveis.Application.Rentals.Commands.GetById;
using LocadoraDeAutomoveis.Application.Rentals.Commands.Return;
using LocadoraDeAutomoveis.Application.Rentals.Commands.Update;
using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Drivers;
using LocadoraDeAutomoveis.Domain.Employees;
using LocadoraDeAutomoveis.Domain.RateServices;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Vehicles;
using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Mappings;

public class RentalProfile : Profile
{
    public RentalProfile()
    {
        // CONTROLLER
        // Update
        CreateMap<(UpdateRentalRequestPartial p, Guid id), UpdateRentalRequest>()
            .ConvertUsing(src => new UpdateRentalRequest(
                src.id,
                src.p.StartDate,
                src.p.ExpectedReturnDate,
                src.p.StartKm,
                src.p.EmployeeId,
                src.p.ClientId,
                src.p.DriverId,
                src.p.VehicleId,
                src.p.CouponId,
                src.p.SelectedPlanType,
                src.p.EstimatedKilometers,
                src.p.RentalRateServicesIds
            ));

        // DTOs
        CreateMap<Employee, RentalEmployeeDto>()
            .ConvertUsing(src => new RentalEmployeeDto(
                src.Id,
                src.FullName
            ));

        CreateMap<Client, RentalClientDto>()
            .ConvertUsing(src => new RentalClientDto(
                src.Id,
                src.FullName
            ));

        CreateMap<Driver, RentalDriverDto>()
            .ConvertUsing(src => new RentalDriverDto(
                src.Id,
                src.FullName
            ));

        CreateMap<Vehicle, RentalVehicleDto>()
            .ConvertUsing(src => new RentalVehicleDto(
                src.Id,
                src.LicensePlate
            ));

        CreateMap<Rental, RentalDto>()
            .ConvertUsing((src, dest, ctx) => new RentalDto(
                src.Id,
                (src.Employee is not null) ? ctx.Mapper.Map<RentalEmployeeDto>(src.Employee) : null,
                ctx.Mapper.Map<RentalClientDto>(src.Client),
                ctx.Mapper.Map<RentalDriverDto>(src.Driver),
                ctx.Mapper.Map<RentalVehicleDto>(src.Vehicle),
                src.SelectedPlanType,
                src.StartDate,
                src.ExpectedReturnDate,
                src.ReturnDate,
                src.BaseRentalPrice,
                src.FinalPrice,
                src.RateServices.Count
            ));

        CreateMap<RateService, RentalRateServiceDto>()
            .ConvertUsing(src => new RentalRateServiceDto(
                src.Id,
                src.Name
            ));

        CreateMap<Rental, ByIdRentalDto>()
            .ConvertUsing((src, dest, ctx) => new ByIdRentalDto(
                src.Id,
                (src.Employee is not null) ? ctx.Mapper.Map<RentalEmployeeDto>(src.Employee) : null,
                ctx.Mapper.Map<RentalClientDto>(src.Client),
                ctx.Mapper.Map<RentalDriverDto>(src.Driver),
                ctx.Mapper.Map<RentalVehicleDto>(src.Vehicle),
                src.SelectedPlanType,
                src.StartDate,
                src.ExpectedReturnDate,
                src.ReturnDate,
                src.BaseRentalPrice,
                src.FinalPrice,
                src.RateServices.Count,
                src.RateServices.Select(r => ctx.Mapper.Map<RentalRateServiceDto>(r)).ToImmutableList()
                ?? ImmutableList<RentalRateServiceDto>.Empty
            ));

        // HANDLERS
        // Create
        CreateMap<CreateRentalRequest, Rental>()
            .ConvertUsing(src => new Rental(
                src.StartDate,
                src.ExpectedReturnDate,
                src.StartKm
            ));

        // GetALl
        CreateMap<List<Rental>, GetAllRentalResponse>()
            .ConvertUsing((src, dest, ctx) => new GetAllRentalResponse(
                src.Count,
                src.Select(c => ctx.Mapper.Map<RentalDto>(c)).ToImmutableList()
                    ?? ImmutableList<RentalDto>.Empty
            ));

        // GetById
        CreateMap<Rental, GetByIdRentalResponse>()
            .ConvertUsing((src, dest, ctx) => new GetByIdRentalResponse(
               ctx.Mapper.Map<ByIdRentalDto>(src)
            ));

        // Return
        CreateMap<(ReturnRentalRequest r, DateTimeOffset d, decimal km), RentalReturn>()
            .ConvertUsing(src => new RentalReturn(
                src.d,
                src.r.EndKm,
                src.km
            ));

        // Update
        CreateMap<UpdateRentalRequest, Rental>()
            .ConvertUsing(src => new Rental(
                src.StartDate,
                src.ExpectedReturnDate,
                src.StartKm
            )
            { Id = src.Id });
    }
}
