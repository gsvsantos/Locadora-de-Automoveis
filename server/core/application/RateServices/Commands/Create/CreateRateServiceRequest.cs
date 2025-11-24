using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.RateServices.Commands.Create;

public record CreateRateServiceRequest(
    string Name,
    decimal Price,
    bool IsFixed
) : IRequest<Result<CreateRateServiceResponse>>;
