using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.RentalExtras.Commands.GetAll;

public record GetAllRentalExtraRequestPartial(
    int? Quantity,
    bool? IsActive
);

public record GetAllRentalExtraRequest(
    int? Quantity,
    bool? IsActive
) : IRequest<Result<GetAllRentalExtraResponse>>;