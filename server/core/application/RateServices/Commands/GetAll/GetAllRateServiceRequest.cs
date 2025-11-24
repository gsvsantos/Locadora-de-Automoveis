using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.RateServices.Commands.GetAll;

public record GetAllRateServiceRequest(
    int? Quantity
) : IRequest<Result<GetAllRateServiceResponse>>;