using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Rentals.Commands.GetMyRentalStatus;

public record GetMyRentalStatusRequest(
) : IRequest<Result<GetMyRentalStatusResponse>>;
