using AutoMapper;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.BillingPlans;
using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.BillingPlans.Commands.Update;

public class UpdateBillingPlanRequestHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IRepositoryBillingPlan repositoryBillingPlan,
    IRepositoryGroup repositoryGroup,
    IValidator<BillingPlan> validator,
    ILogger<UpdateBillingPlanRequestHandler> logger
) : IRequestHandler<UpdateBillingPlanRequest, Result<UpdateBillingPlanResponse>>
{
    public async Task<Result<UpdateBillingPlanResponse>> Handle(
        UpdateBillingPlanRequest request, CancellationToken cancellationToken)
    {
        BillingPlan? selectedBillingPlan = await repositoryBillingPlan.GetByIdAsync(request.Id);

        if (selectedBillingPlan is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(request.Id));
        }

        Group? selectedGroup = selectedBillingPlan.GroupId != request.GroupId
                ? await repositoryGroup.GetByIdAsync(request.GroupId)
                : selectedBillingPlan.Group;

        if (selectedGroup is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(request.GroupId));
        }

        BillingPlan updatedBillingPlan = mapper.Map<BillingPlan>((request, selectedGroup));

        try
        {
            ValidationResult validationResult = await validator.ValidateAsync(updatedBillingPlan, cancellationToken);

            if (!validationResult.IsValid)
            {
                List<string> errors = validationResult.Errors
                    .Select(failure => failure.ErrorMessage)
                    .ToList();

                return Result.Fail(ErrorResults.BadRequestError(errors));
            }

            selectedBillingPlan.AssociateGroup(selectedGroup);

            List<BillingPlan> existingBillingPlans = await repositoryBillingPlan.GetAllAsync();

            if (GroupAlreadyHaveBillingPlan(updatedBillingPlan, existingBillingPlans))
            {
                return Result.Fail(BillingPlanErrorResults.GroupAlreadyHaveBillingPlanError(selectedGroup.Name));
            }

            await repositoryBillingPlan.UpdateAsync(selectedBillingPlan.Id, updatedBillingPlan);

            await unitOfWork.CommitAsync();

            return Result.Ok(new UpdateBillingPlanResponse(selectedBillingPlan.Id));
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

    private static bool GroupAlreadyHaveBillingPlan(BillingPlan BillingPlan, List<BillingPlan> existingBillingPlans)
    {
        return existingBillingPlans
            .Any(entity =>
            entity.Id != BillingPlan.Id &&
            string.Equals(
                entity.Group.Name,
                BillingPlan.Group.Name,
                StringComparison.CurrentCultureIgnoreCase)
            );
    }
}
