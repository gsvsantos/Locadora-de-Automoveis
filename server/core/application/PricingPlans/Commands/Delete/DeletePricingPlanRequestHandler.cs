using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.PricingPlans;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.PricingPlans.Commands.Delete;

public class DeletePricingPlanRequestHandler(
    IUnitOfWork unitOfWork,
    IRepositoryPricingPlan repositoryPricingPlan,
    IRepositoryRental repositoryRental,
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

        bool hasActiveRentals = await repositoryRental.HasActiveRentalsByPricingPlan(request.Id);
        if (hasActiveRentals)
        {
            return Result.Fail(ErrorResults.BadRequestError("Cannot remove a pricing plan associated with active rentals."));
        }

        try
        {
            bool hasHistory = await repositoryRental.HasRentalHistoryByPricingPlan(request.Id);

            if (hasHistory)
            {
                selectedPricingPlan.Deactivate();

                await repositoryPricingPlan.UpdateAsync(selectedPricingPlan.Id, selectedPricingPlan);

                logger.LogInformation(
                    "Pricing Plan '{@PlanName}' ({@PlanId}) was deactivated instead of deleted to preserve rental history.",
                    selectedPricingPlan.Name,
                    request.Id
                );
            }
            else
            {
                await repositoryPricingPlan.DeleteAsync(request.Id);

                logger.LogInformation(
                    "Pricing Plan '{@PlanName}' ({@PlanId}) was permanently deleted.",
                    selectedPricingPlan.Name,
                    request.Id
                );
            }

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
