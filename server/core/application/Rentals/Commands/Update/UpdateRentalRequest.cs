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
    Guid? CouponId,
    EBillingPlanType BillingPlanType,
    decimal? EstimatedKilometers,
    List<Guid> RentalRentalExtrasIds
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
    Guid? CouponId,
    EBillingPlanType BillingPlanType,
    decimal? EstimatedKilometers,
    List<Guid> RentalRentalExtrasIds
) : IRequest<Result<UpdateRentalResponse>>;
