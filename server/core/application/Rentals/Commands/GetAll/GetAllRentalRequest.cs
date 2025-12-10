using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Rentals.Commands.GetAll;

public record GetAllRentalRequestPartial(
    int? Quantity,
    bool? IsActive
);

public record GetAllRentalRequest(
    int? Quantity,
    bool? IsActive
) : IRequest<Result<GetAllRentalResponse>>;
