using AutoMapper;
using LocadoraDeAutomoveis.Application.Partners.Commands.GetAll;
using LocadoraDeAutomoveis.Application.Rentals.Commands.Create;
using LocadoraDeAutomoveis.Application.Rentals.Commands.GetAll;
using LocadoraDeAutomoveis.Application.Rentals.Commands.GetById;
using LocadoraDeAutomoveis.Application.Rentals.Commands.Return;
using LocadoraDeAutomoveis.Application.Rentals.Commands.Update;
using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Coupons;
using LocadoraDeAutomoveis.Domain.Drivers;
using LocadoraDeAutomoveis.Domain.Employees;
using LocadoraDeAutomoveis.Domain.RentalExtras;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Vehicles;
using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Mappings;

public class RentalProfile : Profile
{
    public RentalProfile()
    {
        // CONTROLLER
        // GetAll
        CreateMap<GetAllRentalRequestPartial, GetAllRentalRequest>()
            .ConvertUsing(src => new GetAllRentalRequest(
                src.Quantity,
                src.IsActive
            ));

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
                src.p.BillingPlanType,
                src.p.EstimatedKilometers,
                src.p.RentalRentalExtrasIds
            ));

        // Return
        CreateMap<(ReturnRentalRequestPartial p, Guid id), ReturnRentalRequest>()
            .ConvertUsing(src => new ReturnRentalRequest(
                src.id,
                src.p.EndKm,
                src.p.FuelLevel
            ));

        // DTOs
        CreateMap<Employee, RentalEmployeeDto>()
            .ConvertUsing(src => new RentalEmployeeDto(
                src.Id,
                src.FullName,
                src.IsActive
            ));

        CreateMap<Client, RentalClientDto>()
            .ConvertUsing(src => new RentalClientDto(
                src.Id,
                src.FullName,
                src.IsActive
            ));

        CreateMap<Driver, RentalDriverDto>()
            .ConvertUsing(src => new RentalDriverDto(
                src.Id,
                src.FullName,
                src.IsActive
            ));

        CreateMap<Vehicle, RentalVehicleDto>()
            .ConvertUsing(src => new RentalVehicleDto(
                src.Id,
                src.LicensePlate,
                src.IsActive
            ));

        CreateMap<Coupon, RentalCouponDto>()
            .ConvertUsing((src, dest, ctx) => new RentalCouponDto(
                src.Id,
                src.Name,
                ctx.Mapper.Map<PartnerDto>(src.Partner),
                src.IsActive
            ));

        CreateMap<Rental, RentalDto>()
            .ConvertUsing((src, dest, ctx) => new RentalDto(
                src.Id,
                (src.Employee is not null) ? ctx.Mapper.Map<RentalEmployeeDto>(src.Employee) : null,
                ctx.Mapper.Map<RentalClientDto>(src.Client),
                ctx.Mapper.Map<RentalDriverDto>(src.Driver),
                ctx.Mapper.Map<RentalVehicleDto>(src.Vehicle),
                ctx.Mapper.Map<RentalCouponDto>(src.Coupon),
                src.StartDate,
                src.ExpectedReturnDate,
                src.StartKm,
                src.BillingPlanType,
                src.ReturnDate,
                src.BaseRentalPrice,
                src.FinalPrice,
                src.EstimatedKilometers,
                src.Extras.Count,
                src.IsActive
            ));

        CreateMap<RentalExtra, RentalRentalExtraDto>()
            .ConvertUsing(src => new RentalRentalExtraDto(
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
                ctx.Mapper.Map<RentalCouponDto>(src.Coupon),
                src.StartDate,
                src.ExpectedReturnDate,
                src.StartKm,
                src.BillingPlanType,
                src.ReturnDate,
                src.BaseRentalPrice,
                src.FinalPrice,
                src.EstimatedKilometers,
                src.Extras.Count,
                src.Extras.Select(r => ctx.Mapper.Map<RentalRentalExtraDto>(r)).ToImmutableList()
                ?? ImmutableList<RentalRentalExtraDto>.Empty,
                src.IsActive
            ));

        // HANDLERS
        // Create
        CreateMap<CreateRentalRequest, Rental>()
            .ConvertUsing(src => new Rental(
                src.StartDate,
                src.ExpectedReturnDate,
                src.StartKm
            ));

        // GetAll
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
