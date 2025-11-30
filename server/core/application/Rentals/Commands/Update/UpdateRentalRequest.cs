using FluentResults;
using LocadoraDeAutomoveis.Domain.Rentals;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Rentals.Commands.Update;

public record UpdateRentalRequestPartial(
    DateTimeOffset StartDate,
    DateTimeOffset ExpectedReturnDate,
    decimal StartKm,
    Guid? EmployeeId,
    Guid ClientId,
    Guid DriverId,
    Guid VehicleId,
    EPricingPlanType SelectedPlanType,
    List<Guid> RentalRateServicesIds
);

public record UpdateRentalRequest(
    Guid Id,
    DateTimeOffset StartDate,
    DateTimeOffset ExpectedReturnDate,
    decimal StartKm,
    Guid? EmployeeId,
    Guid ClientId,
    Guid DriverId,
    Guid VehicleId,
    EPricingPlanType SelectedPlanType,
    List<Guid> RentalRateServicesIds
) : IRequest<Result<UpdateRentalResponse>>;
