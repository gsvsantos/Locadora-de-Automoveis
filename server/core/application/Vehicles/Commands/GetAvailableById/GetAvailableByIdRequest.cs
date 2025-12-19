using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Vehicles.Commands.GetAvailableById;

public record GetAvailableByIdRequest(
    Guid Id
) : IRequest<Result<GetAvailableByIdResponse>>;
