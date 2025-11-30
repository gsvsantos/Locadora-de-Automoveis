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
    EPricingPlanType SelectedPlanType,
    DateTimeOffset StartDate,
    DateTimeOffset ExpectedReturnDate,
    DateTimeOffset? ReturnDate,
    decimal BaseRentalPrice,
    decimal FinalPrice
);

public record RentalEmployeeDto(
    Guid Id,
    string FullName
);

public record RentalClientDto(
    Guid Id,
    string FullName
);

public record RentalDriverDto(
    Guid Id,
    string FullName
);

public record RentalVehicleDto(
    Guid Id,
    string LicensePlate
);