using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.PricingPlans.Commands.GetAll;

public record GetAllPricingPlanRequest(
    int? Quantity
) : IRequest<Result<GetAllPricingPlanResponse>>;
