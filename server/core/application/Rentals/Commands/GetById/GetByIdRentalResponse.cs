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
    EPricingPlanType SelectedPlanType,
    DateTimeOffset StartDate,
    DateTimeOffset ExpectedReturnDate,
    DateTimeOffset? ReturnDate,
    decimal BaseRentalPrice,
    decimal FinalPrice,
    ImmutableList<RentalRateServiceDto> RateServices
) : RentalDto(
    Id,
    Employee,
    Client,
    Driver,
    Vehicle,
    SelectedPlanType,
    StartDate,
    ExpectedReturnDate,
    ReturnDate,
    BaseRentalPrice,
    FinalPrice
);

public record RentalRateServiceDto(
    Guid Id,
    string Name
);