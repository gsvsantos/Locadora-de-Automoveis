using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.PricingPlans;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.PricingPlans.Commands.Delete;

public class DeletePricingPlanRequestHandler(
    IUnitOfWork unitOfWork,
    IRepositoryPricingPlan repositoryPricingPlan,
    ILogger<DeletePricingPlanRequestHandler> logger
) : IRequestHandler<DeletePricingPlanRequest, Result<DeletePricingPlanResponse>>
{
    public async Task<Result<DeletePricingPlanResponse>> Handle(
        DeletePricingPlanRequest request, CancellationToken cancellationToken)
    {
        PricingPlan? selectedPricingPlan = await repositoryPricingPlan.GetByIdAsync(request.Id);

        if (selectedPricingPlan is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(request.Id));
        }

        try
        {
            await repositoryPricingPlan.DeleteAsync(request.Id);

            await unitOfWork.CommitAsync();

            return Result.Ok(new DeletePricingPlanResponse());
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync();

            logger.LogError(
                ex,
                "An error occurred during the request. \n{@Request}.", request
            );

            return Result.Fail(ErrorResults.InternalServerError(ex));
        }
    }
}
