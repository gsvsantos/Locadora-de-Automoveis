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
    EBillingPlanType SelectedPlanType,
    DateTimeOffset StartDate,
    DateTimeOffset ExpectedReturnDate,
    DateTimeOffset? ReturnDate,
    decimal BaseRentalPrice,
    decimal FinalPrice,
    int RentalExtrasQuantity,
    ImmutableList<RentalRentalExtraDto> RentalExtras
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
    FinalPrice,
    RentalExtrasQuantity
);

public record RentalRentalExtraDto(
    Guid Id,
    string Name
);