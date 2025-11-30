using FluentResults;
using LocadoraDeAutomoveis.Domain.Rentals;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Rentals.Commands.Create;

public record CreateRentalRequest(
    DateTimeOffset StartDate,
    DateTimeOffset ExpectedReturnDate,
    decimal StartKm,
    Guid? EmployeeId,
    Guid ClientId,
    Guid DriverId,
    Guid VehicleId,
    EPricingPlanType SelectedPlanType,
    List<Guid> RentalRateServicesIds
) : IRequest<Result<CreateRentalResponse>>;
