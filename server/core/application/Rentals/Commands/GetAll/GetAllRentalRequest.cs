using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Rentals.Commands.GetAll;

public record GetAllRentalRequest(
    int? Quantity
) : IRequest<Result<GetAllRentalResponse>>;
