using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.BillingPlans;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.BillingPlans.Commands.GetById;

public class GetByIdBillingPlanRequestHandler(
    IMapper mapper,
    IRepositoryBillingPlan repositoryBillingPlan,
    ILogger<GetByIdBillingPlanRequestHandler> logger
) : IRequestHandler<GetByIdBillingPlanRequest, Result<GetByIdBillingPlanResponse>>
{
    public async Task<Result<GetByIdBillingPlanResponse>> Handle(
        GetByIdBillingPlanRequest request, CancellationToken cancellationToken)
    {
        try
        {
            BillingPlan? selectedBillingPlan = await repositoryBillingPlan.GetByIdAsync(request.Id);

            if (selectedBillingPlan is null)
            {
                return Result.Fail(ErrorResults.NotFoundError(request.Id));
            }

            GetByIdBillingPlanResponse response = mapper.Map<GetByIdBillingPlanResponse>(selectedBillingPlan);

            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error retrieving Billing Plan by ID {BillingPlanId}", request.Id);

            return Result.Fail(new Error("An error occurred while retrieving the billing plan."));
        }
    }
}
