using LocadoraDeAutomoveis.Application.Partners.Commands.GetCoupons;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Vehicles;
using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Rentals.Commands.GetAll;

public record GetAllRentalResponse(
    int Quantity,
    ImmutableList<RentalDto> Rentals
);

public record RentalDto(
    Guid Id,
    RentalEmployeeDto? Employee,
    RentalClientDto Client,
    RentalDriverDto Driver,
    RentalVehicleDto Vehicle,
    CouponDto? Coupon,
    DateTimeOffset StartDate,
    DateTimeOffset ExpectedReturnDate,
    decimal StartKm,
    EBillingPlanType BillingPlanType,
    DateTimeOffset? ReturnDate,
    decimal BaseRentalPrice,
    decimal FinalPrice,
    decimal? EstimatedKilometers,
    int RentalExtrasQuantity,
    bool IsActive
);

public record RentalEmployeeDto(
    Guid Id,
    string FullName,
    bool IsActive
);

public record RentalClientDto(
    Guid Id,
    string FullName,
    bool IsActive
);

public record RentalDriverDto(
    Guid Id,
    string FullName,
    bool IsActive
);

public record RentalVehicleDto(
    Guid Id,
    string LicensePlate,
    EFuelType FuelType,
    int FuelTankCapacity,
    bool IsActive
);
