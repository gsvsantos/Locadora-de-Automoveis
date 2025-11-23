using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.PricingPlans.Commands.Delete;

public record DeletePricingPlanRequest(
    Guid Id
) : IRequest<Result<DeletePricingPlanResponse>>;