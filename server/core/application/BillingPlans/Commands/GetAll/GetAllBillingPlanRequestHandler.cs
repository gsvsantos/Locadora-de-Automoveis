using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.BillingPlans;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.BillingPlans.Commands.GetAll;

public class GetAllBillingPlanRequestHandler(
    IMapper mapper,
    IRepositoryBillingPlan repositoryBillingPlan,
    ILogger<GetAllBillingPlanRequestHandler> logger
) : IRequestHandler<GetAllBillingPlanRequest, Result<GetAllBillingPlanResponse>>
{
    public async Task<Result<GetAllBillingPlanResponse>> Handle(
        GetAllBillingPlanRequest request, CancellationToken cancellationToken)
    {
        try
        {
            List<BillingPlan> billingPlans = [];
            bool quantityProvided = request.Quantity.HasValue && request.Quantity.Value > 0;
            bool inactiveProvided = request.IsActive.HasValue;

            if (quantityProvided && inactiveProvided)
            {
                int quantity = request.Quantity!.Value;
                bool isActive = request.IsActive!.Value;

                billingPlans = await repositoryBillingPlan.GetAllAsync(quantity, isActive);
            }
            else if (inactiveProvided)
            {
                bool isActive = request.IsActive!.Value;

                billingPlans = await repositoryBillingPlan.GetAllAsync(isActive);
            }
            else if (quantityProvided)
            {
                int quantity = request.Quantity!.Value;

                billingPlans = await repositoryBillingPlan.GetAllAsync(quantity);
            }
            else
            {
                billingPlans = await repositoryBillingPlan.GetAllAsync(true);
            }

            GetAllBillingPlanResponse response = mapper.Map<GetAllBillingPlanResponse>(billingPlans);

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
