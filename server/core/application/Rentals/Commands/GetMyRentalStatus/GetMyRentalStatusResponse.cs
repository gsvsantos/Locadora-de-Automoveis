namespace LocadoraDeAutomoveis.Application.Rentals.Commands.GetMyRentalStatus;

public record GetMyRentalStatusResponse(
    bool CanRent,
    string? Reason,
    ActiveRentalDto? Rental
);

public sealed record ActiveRentalDto(
    Guid RentalId,
    string VehicleLicensePlate,
    DateTimeOffset StartedAt
);