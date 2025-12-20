using AutoMapper;
using LocadoraDeAutomoveis.Application.BillingPlans.Commands.GetAll;
using LocadoraDeAutomoveis.Application.Partners.Commands.GetCoupons;
using LocadoraDeAutomoveis.Application.RentalExtras.Commands.GetAll;
using LocadoraDeAutomoveis.Application.Rentals.Commands.Create;
using LocadoraDeAutomoveis.Application.Rentals.Commands.CreateSelfRental;
using LocadoraDeAutomoveis.Application.Rentals.Commands.GetAll;
using LocadoraDeAutomoveis.Application.Rentals.Commands.GetById;
using LocadoraDeAutomoveis.Application.Rentals.Commands.GetMyRentals;
using LocadoraDeAutomoveis.Application.Rentals.Commands.Return;
using LocadoraDeAutomoveis.Application.Rentals.Commands.Update;
using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Drivers;
using LocadoraDeAutomoveis.Domain.Employees;
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
                src.p.FuelLevelAtReturn
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
                src.FuelType,
                src.FuelTankCapacity,
                src.IsActive
            ));

        CreateMap<RentalReturn, RentalReturnDto>()
            .ConvertUsing(src => new RentalReturnDto(
                src.ReturnDate,
                src.EndKm,
                src.TotalMileage,
                src.ExtrasTotalCost,
                src.FuelPenalty,
                src.PenaltyTotalCost,
                src.DiscountTotal,
                src.FinalPrice,
                src.FuelLevelAtReturn
            ));

        CreateMap<Rental, RentalDto>()
            .ConvertUsing((src, dest, ctx) => new RentalDto(
                src.Id,
                (src.Employee is not null) ? ctx.Mapper.Map<RentalEmployeeDto>(src.Employee) : null,
                ctx.Mapper.Map<RentalClientDto>(src.Client),
                ctx.Mapper.Map<RentalDriverDto>(src.Driver),
                ctx.Mapper.Map<RentalVehicleDto>(src.Vehicle),
                (src.Coupon is not null) ? ctx.Mapper.Map<CouponDto>(src.Coupon) : null,
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

        CreateMap<Rental, ByIdRentalDto>()
            .ConvertUsing((src, dest, ctx) => new ByIdRentalDto(
                src.Id,
                (src.Employee is not null) ? ctx.Mapper.Map<RentalEmployeeDto>(src.Employee) : null,
                ctx.Mapper.Map<RentalClientDto>(src.Client),
                ctx.Mapper.Map<RentalDriverDto>(src.Driver),
                ctx.Mapper.Map<RentalVehicleDto>(src.Vehicle),
                (src.Coupon is not null) ? ctx.Mapper.Map<CouponDto>(src.Coupon) : null,
                src.StartDate,
                src.ExpectedReturnDate,
                src.StartKm,
                src.BillingPlanType,
                ctx.Mapper.Map<BillingPlanDto>(src.BillingPlan),
                src.ReturnDate,
                (src.RentalReturn is not null) ? ctx.Mapper.Map<RentalReturnDto>(src.RentalReturn) : null,
                src.BaseRentalPrice,
                src.FinalPrice,
                src.EstimatedKilometers,
                src.Extras.Count,
                src.Extras.Select(r => ctx.Mapper.Map<RentalExtraDto>(r)).ToImmutableList()
                ?? ImmutableList<RentalExtraDto>.Empty,
                src.IsActive
            ));

        CreateMap<Rental, ClientRentalDto>()
            .ConvertUsing((src, dest, ctx) => new ClientRentalDto(
                src.Id,
                ctx.Mapper.Map<RentalClientDto>(src.Client),
                ctx.Mapper.Map<RentalDriverDto>(src.Driver),
                ctx.Mapper.Map<RentalVehicleDto>(src.Vehicle),
                src.StartDate,
                src.ExpectedReturnDate,
                src.StartKm,
                src.BillingPlanType,
                src.ReturnDate,
                src.BaseRentalPrice,
                src.FinalPrice,
                src.EstimatedKilometers,
                src.Extras.Count,
                src.IsActive,
                src.Status,
                src.GetTenantId()
            ));

        // HANDLERS
        // Create
        CreateMap<CreateRentalRequest, Rental>()
            .ConvertUsing(src => new Rental(
                src.StartDate,
                src.ExpectedReturnDate
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
                src.ExpectedReturnDate
            )
            { Id = src.Id });

        // CreateSelf
        CreateMap<CreateSelfRentalRequest, Rental>()
            .ConvertUsing(src => new Rental(
                src.StartDate,
                src.ExpectedReturnDate
            ));
    }
}
