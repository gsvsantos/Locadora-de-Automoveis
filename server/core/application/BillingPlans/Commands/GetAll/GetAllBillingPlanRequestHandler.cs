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
            List<BillingPlan> BillingPlans =
                request.Quantity.HasValue && request.Quantity.Value > 0
                ? await repositoryBillingPlan.GetAllAsync(request.Quantity.Value)
                : await repositoryBillingPlan.GetAllAsync();

            GetAllBillingPlanResponse response = mapper.Map<GetAllBillingPlanResponse>(BillingPlans);

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
