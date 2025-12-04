using FluentResults;
using LocadoraDeAutomoveis.Domain.RentalExtras;
using MediatR;

namespace LocadoraDeAutomoveis.Application.RentalExtras.Commands.Create;

public record CreateRentalExtraRequest(
    string Name,
    decimal Price,
    bool IsFixed,
    EExtraType Type
) : IRequest<Result<CreateRentalExtraResponse>>;
