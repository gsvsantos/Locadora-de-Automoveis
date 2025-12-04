using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.RentalExtras.Commands.GetAll;

public record GetAllRentalExtraRequest(
    int? Quantity
) : IRequest<Result<GetAllRentalExtraResponse>>;