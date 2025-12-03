using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.PricingPlans;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.PricingPlans.Commands.GetById;

public class GetByIdPricingPlanRequestHandler(
    IMapper mapper,
    IRepositoryPricingPlan repositoryPricingPlan,
    ILogger<GetByIdPricingPlanRequestHandler> logger
) : IRequestHandler<GetByIdPricingPlanRequest, Result<GetByIdPricingPlanResponse>>
{
    public async Task<Result<GetByIdPricingPlanResponse>> Handle(
        GetByIdPricingPlanRequest request, CancellationToken cancellationToken)
    {
        try
        {
            PricingPlan? selectedPricingPlan = await repositoryPricingPlan.GetByIdAsync(request.Id);

            if (selectedPricingPlan is null)
            {
                return Result.Fail(ErrorResults.NotFoundError(request.Id));
            }

            GetByIdPricingPlanResponse response = mapper.Map<GetByIdPricingPlanResponse>(selectedPricingPlan);

            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error retrieving Pricing Plan by ID {PricingPlanId}", request.Id);

            return Result.Fail(new Error("An error occurred while retrieving the pricing plan."));
        }
    }
}
