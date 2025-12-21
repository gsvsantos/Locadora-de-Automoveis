using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Rentals.Commands.GetMyRentalBy;

public record GetMyRentalByIdRequest(
    Guid Id
) : IRequest<Result<GetMyRentalByIdResponse>>;
