using FluentResults;
using LocadoraDeAutomoveis.Domain.Rentals;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Rentals.Commands.GetMyRentals;

public record GetMyRentalsRequest(
    string? Term = null,
    ERentalStatus? Status = null,
    Guid? TenantId = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<Result<GetMyRentalsResponse>>;
