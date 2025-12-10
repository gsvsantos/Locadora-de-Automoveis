using LocadoraDeAutomoveis.Application.Rentals.Commands.GetAll;
using LocadoraDeAutomoveis.Domain.Rentals;
using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Rentals.Commands.GetById;

public record GetByIdRentalResponse(ByIdRentalDto Rental);

public record ByIdRentalDto(
    Guid Id,
    RentalEmployeeDto? Employee,
    RentalClientDto Client,
    RentalDriverDto Driver,
    RentalVehicleDto Vehicle,
    RentalCouponDto Coupon,
    DateTimeOffset StartDate,
    DateTimeOffset ExpectedReturnDate,
    decimal StartKm,
    EBillingPlanType BillingPlanType,
    DateTimeOffset? ReturnDate,
    decimal BaseRentalPrice,
    decimal FinalPrice,
    decimal? EstimatedKilometers,
    int RentalExtrasQuantity,
    ImmutableList<RentalRentalExtraDto> RentalExtras,
    bool IsActive
) : RentalDto(
    Id,
    Employee,
    Client,
    Driver,
    Vehicle,
    Coupon,
    StartDate,
    ExpectedReturnDate,
    StartKm,
    BillingPlanType,
    ReturnDate,
    BaseRentalPrice,
    FinalPrice,
    EstimatedKilometers,
    RentalExtrasQuantity,
    IsActive
);

public record RentalRentalExtraDto(
    Guid Id,
    string Name
);