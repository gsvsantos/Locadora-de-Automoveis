using FluentResults;
using LocadoraDeAutomoveis.Domain.RateServices;
using MediatR;

namespace LocadoraDeAutomoveis.Application.RateServices.Commands.Create;

public record CreateRateServiceRequest(
    string Name,
    decimal Price,
    bool IsFixed,
    ERateType RateType
) : IRequest<Result<CreateRateServiceResponse>>;
