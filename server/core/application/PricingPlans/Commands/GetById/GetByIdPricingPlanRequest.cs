using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.PricingPlans.Commands.GetById;

public record GetByIdPricingPlanRequest(
    Guid Id
) : IRequest<Result<GetByIdPricingPlanResponse>>;