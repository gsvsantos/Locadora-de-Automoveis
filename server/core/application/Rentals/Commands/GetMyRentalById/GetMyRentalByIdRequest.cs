using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Rentals.Commands.GetMyRentalById;

public record GetMyRentalByIdRequest(
    Guid Id
) : IRequest<Result<GetMyRentalByIdResponse>>;
