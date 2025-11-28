using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.PricingPlans;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.PricingPlans.Commands.GetAll;

public class GetAllPricingPlanRequestHandler(
    IRepositoryPricingPlan repositoryPricingPlan,
    ILogger<GetAllPricingPlanRequestHandler> logger
) : IRequestHandler<GetAllPricingPlanRequest, Result<GetAllPricingPlanResponse>>
{
    public async Task<Result<GetAllPricingPlanResponse>> Handle(
        GetAllPricingPlanRequest request, CancellationToken cancellationToken)
    {
        try
        {
            List<PricingPlan> pricingPlans =
                request.Quantity.HasValue && request.Quantity.Value > 0
                ? await repositoryPricingPlan.GetAllAsync(request.Quantity.Value)
                : await repositoryPricingPlan.GetAllAsync();

            GetAllPricingPlanResponse response = new(
                pricingPlans.Count,
                pricingPlans.Select(pricingPlan => new PricingPlanDto(
                    pricingPlan.Id,
                    $"{pricingPlan.Group.Name} - Pricing Plans",
                    new(pricingPlan.DailyPlan.DailyRate, pricingPlan.DailyPlan.PricePerKm),
                    new(pricingPlan.ControlledPlan.DailyRate, pricingPlan.ControlledPlan.PricePerKmExtrapolated),
                    new(pricingPlan.FreePlan.FixedRate),
                    pricingPlan.GroupId
                )).ToImmutableList()
            );

            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred during the request. \n{@Request}.", request
            );

            return Result.Fail(ErrorResults.InternalServerError(ex));
        }
    }
}
