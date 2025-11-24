using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.RateServices.Commands.GetById;

public record GetByIdRateServiceRequest(
    Guid Id
) : IRequest<Result<GetByIdRateServiceResponse>>;
