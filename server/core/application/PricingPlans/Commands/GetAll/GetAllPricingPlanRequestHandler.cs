using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.PricingPlans;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.PricingPlans.Commands.GetAll;

public class GetAllPricingPlanRequestHandler(
    IMapper mapper,
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

            GetAllPricingPlanResponse response = mapper.Map<GetAllPricingPlanResponse>(pricingPlans);

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
