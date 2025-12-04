using FluentResults;
using LocadoraDeAutomoveis.Domain.RentalExtras;
using MediatR;

namespace LocadoraDeAutomoveis.Application.RentalExtras.Commands.Update;

public record UpdateRentalExtraRequestPartial(
    string Name,
    decimal Price,
    bool IsFixed,
    EExtraType Type
);

public record UpdateRentalExtraRequest(
    Guid Id,
    string Name,
    decimal Price,
    bool IsFixed,
    EExtraType Type
) : IRequest<Result<UpdateRentalExtraResponse>>;
