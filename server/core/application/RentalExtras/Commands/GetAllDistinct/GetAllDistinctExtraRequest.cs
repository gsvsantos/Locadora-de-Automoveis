using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.RentalExtras.Commands.GetAllDistinct;

public record GetAllDistinctExtraRequest(
    Guid VehicleId
) : IRequest<Result<GetAllDistinctExtraResponse>>;