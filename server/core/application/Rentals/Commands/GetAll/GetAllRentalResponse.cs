using LocadoraDeAutomoveis.Application.Vehicles.Commands.GetAll;
using LocadoraDeAutomoveis.Domain.Rentals;
using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Rentals.Commands.GetAll;

public record GetAllRentalResponse(
    int Quantity,
    ImmutableList<RentalDto> Rentals
);

public record RentalDto(
    Guid Id,
    string DriverName,
    VehicleDto Vehicle,
    EPricingPlanType SelectedPlanType,
    DateTimeOffset StartDate,
    DateTimeOffset ExpectedReturnDate,
    DateTimeOffset? ReturnDate,
    decimal BaseRentalPrice,
    decimal FinalPrice
);