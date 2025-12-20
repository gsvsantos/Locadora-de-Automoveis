using LocadoraDeAutomoveis.Application.Rentals.Commands.GetAll;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Application.Rentals.Commands.GetMyRentals;

public record GetMyRentalsResponse(
    PagedResult<ClientRentalDto> Rentals
);

public record ClientRentalDto(
    Guid Id,
    RentalClientDto Client,
    RentalDriverDto Driver,
    RentalVehicleDto Vehicle,
    DateTimeOffset StartDate,
    DateTimeOffset ExpectedReturnDate,
    decimal StartKm,
    EBillingPlanType BillingPlanType,
    DateTimeOffset? ReturnDate,
    decimal BaseRentalPrice,
    decimal FinalPrice,
    decimal? EstimatedKilometers,
    int RentalExtrasQuantity,
    bool IsActive,
    ERentalStatus Status,
    Guid TenantId
);
