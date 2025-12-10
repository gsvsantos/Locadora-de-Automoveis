using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Configurations.Commands.Configure;

public record ConfigureRequest(
    decimal GasolinePrice,
    decimal GasPrice,
    decimal DieselPrice,
    decimal AlcoholPrice
) : IRequest<Result<ConfigureResponse>>;
