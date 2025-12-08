using LocadoraDeAutomoveis.Application.Partners.Commands.GetAll;
using LocadoraDeAutomoveis.Domain.Rentals;
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
    bool IsActive
);

public record RentalCouponDto(
    Guid Id,
    string Name,
    PartnerDto Partner,
    bool IsActive
);
