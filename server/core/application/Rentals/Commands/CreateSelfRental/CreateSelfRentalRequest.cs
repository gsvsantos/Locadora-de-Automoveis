using FluentResults;
using LocadoraDeAutomoveis.Application.Rentals.Commands.Create;
using LocadoraDeAutomoveis.Domain.Rentals;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Rentals.Commands.CreateSelfRental;

public record CreateSelfRentalRequest(
    DateTimeOffset StartDate,
    DateTimeOffset ExpectedReturnDate,
    Guid VehicleId,
    Guid DriverId,
    Guid? CouponId,
    EBillingPlanType BillingPlanType,
    decimal? EstimatedKilometers,
    List<Guid> RentalRentalExtrasIds
) : IRequest<Result<CreateRentalResponse>>;