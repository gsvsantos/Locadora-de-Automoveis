using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.RateServices.Commands.Update;

public record UpdateRateServiceRequestPartial(
    string Name,
    decimal Price,
    bool IsFixed
);

public record UpdateRateServiceRequest(
    Guid Id,
    string Name,
    decimal Price,
    bool IsFixed
) : IRequest<Result<UpdateRateServiceResponse>>;
