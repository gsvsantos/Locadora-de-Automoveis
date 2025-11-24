using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.RateServices.Commands.Delete;

public record DeleteRateServiceRequest(
    Guid Id
) : IRequest<Result<DeleteRateServiceResponse>>;
