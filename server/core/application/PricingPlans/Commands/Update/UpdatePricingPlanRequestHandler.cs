using AutoMapper;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.PricingPlans;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.PricingPlans.Commands.Update;

public class UpdatePricingPlanRequestHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IRepositoryPricingPlan repositoryPricingPlan,
    IRepositoryGroup repositoryGroup,
    IValidator<PricingPlan> validator,
    ILogger<UpdatePricingPlanRequestHandler> logger
) : IRequestHandler<UpdatePricingPlanRequest, Result<UpdatePricingPlanResponse>>
{
    public async Task<Result<UpdatePricingPlanResponse>> Handle(
        UpdatePricingPlanRequest request, CancellationToken cancellationToken)
    {
        PricingPlan? selectedPricingPlan = await repositoryPricingPlan.GetByIdAsync(request.Id);

        if (selectedPricingPlan is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(request.Id));
        }

        Group? selectedGroup = (selectedPricingPlan.GroupId != request.GroupId)
                ? await repositoryGroup.GetByIdAsync(request.GroupId)
                : selectedPricingPlan.Group;

        if (selectedGroup is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(request.GroupId));
        }

        PricingPlan updatedPricingPlan = mapper.Map<PricingPlan>((request, selectedGroup));

        try
        {
            ValidationResult validationResult = await validator.ValidateAsync(updatedPricingPlan, cancellationToken);

            if (!validationResult.IsValid)
            {
                List<string> errors = validationResult.Errors
                    .Select(failure => failure.ErrorMessage)
                    .ToList();

                return Result.Fail(ErrorResults.BadRequestError(errors));
            }

            selectedPricingPlan.AssociateGroup(selectedGroup);

            List<PricingPlan> existingPricingPlans = await repositoryPricingPlan.GetAllAsync();

            if (GroupAlreadyHavePricingPlan(updatedPricingPlan, existingPricingPlans))
            {
                return Result.Fail(PricingPlanErrorResults.GroupAlreadyHavePricingPlanError(selectedGroup.Name));
            }

            await repositoryPricingPlan.UpdateAsync(selectedPricingPlan.Id, updatedPricingPlan);

            await unitOfWork.CommitAsync();

            return Result.Ok(new UpdatePricingPlanResponse(selectedPricingPlan.Id));
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

    private static bool GroupAlreadyHavePricingPlan(PricingPlan pricingPlan, List<PricingPlan> existingPricingPlans)
    {
        return existingPricingPlans
            .Any(entity =>
            entity.Id != pricingPlan.Id &&
            string.Equals(
                entity.Group.Name,
                pricingPlan.Group.Name,
                StringComparison.CurrentCultureIgnoreCase)
            );
    }
}
