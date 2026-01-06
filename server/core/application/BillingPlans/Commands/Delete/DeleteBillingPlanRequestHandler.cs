using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.BillingPlans;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.BillingPlans.Commands.Delete;

public class DeleteBillingPlanRequestHandler(
    IUnitOfWork unitOfWork,
    IRepositoryBillingPlan repositoryBillingPlan,
    IRepositoryRental repositoryRental,
    IDistributedCache cache,
    ILogger<DeleteBillingPlanRequestHandler> logger
) : IRequestHandler<DeleteBillingPlanRequest, Result<DeleteBillingPlanResponse>>
{
    public async Task<Result<DeleteBillingPlanResponse>> Handle(
        DeleteBillingPlanRequest request, CancellationToken cancellationToken)
    {
        BillingPlan? selectedBillingPlan = await repositoryBillingPlan.GetByIdAsync(request.Id);

        if (selectedBillingPlan is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(request.Id));
        }

        bool hasActiveRentals = await repositoryRental.HasActiveRentalsByBillingPlan(request.Id);
        if (hasActiveRentals)
        {
            return Result.Fail(ErrorResults.BadRequestError("Cannot remove a billing plan associated with active rentals."));
        }

        try
        {
            bool hasHistory = await repositoryRental.HasRentalHistoryByBillingPlan(request.Id);

            if (hasHistory)
            {
                selectedBillingPlan.Deactivate();

                await repositoryBillingPlan.UpdateAsync(selectedBillingPlan.Id, selectedBillingPlan);

                logger.LogInformation(
                    "Billing Plan '{@PlanName}' ({@PlanId}) was deactivated instead of deleted to preserve rental history.",
                    selectedBillingPlan.Name,
                    request.Id
                );
            }
            else
            {
                await repositoryBillingPlan.DeleteAsync(request.Id);

                logger.LogInformation(
                    "Billing Plan '{@PlanName}' ({@PlanId}) was permanently deleted.",
                    selectedBillingPlan.Name,
                    request.Id
                );
            }

            await unitOfWork.CommitAsync();

            await cache.SetStringAsync("billingPlans:master-version", Guid.NewGuid().ToString(), cancellationToken);

            return Result.Ok(new DeleteBillingPlanResponse());
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
