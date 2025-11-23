using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.PricingPlans;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.PricingPlans.Commands.Create;

public class CreatePricingPlanRequestHandler(
    UserManager<User> userManager,
    IUnitOfWork unitOfWork,
    IRepositoryPricingPlan repositoryPricingPlan,
    IRepositoryGroup repositoryGroup,
    ITenantProvider tenantProvider,
    IUserContext userContext,
    IValidator<PricingPlan> validator,
    ILogger<CreatePricingPlanRequestHandler> logger
) : IRequestHandler<CreatePricingPlanRequest, Result<CreatePricingPlanResponse>>
{
    public async Task<Result<CreatePricingPlanResponse>> Handle(
        CreatePricingPlanRequest request, CancellationToken cancellationToken)
    {
        User? user = await userManager.FindByIdAsync(userContext.GetUserId().ToString());

        if (user is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(userContext.GetUserId()));
        }

        Group? selectedGroup = await repositoryGroup.GetByIdAsync(request.GroupId);

        if (selectedGroup is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(request.GroupId));
        }

        PricingPlan pricingPlan = new(
            request.DailyPlan.ToProps(),
            request.ControlledPlan.ToProps(),
            request.FreePlan.ToProps()
        );

        try
        {
            ValidationResult validationResult = await validator.ValidateAsync(pricingPlan, cancellationToken);

            if (!validationResult.IsValid)
            {
                List<string> errors = validationResult.Errors
                    .Select(failure => failure.ErrorMessage)
                    .ToList();

                return Result.Fail(ErrorResults.BadRequestError(errors));
            }

            List<PricingPlan> existingPricingPlans = await repositoryPricingPlan.GetAllAsync();

            if (GroupAlreadyHavePricingPlan(pricingPlan, existingPricingPlans))
            {
                return Result.Fail(PricingPlanErrorResults.GroupAlreadyHavePricingPlanError(selectedGroup.Name));
            }

            pricingPlan.AssociateTenant(tenantProvider.GetTenantId());

            pricingPlan.AssociateUser(user);

            pricingPlan.AssociateGroup(selectedGroup);

            await repositoryPricingPlan.AddAsync(pricingPlan);

            await unitOfWork.CommitAsync();

            return Result.Ok(new CreatePricingPlanResponse(pricingPlan.Id));
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync();

            logger.LogError(
                ex,
                "An error occurred during registration. \n{@Request}.", request
            );

            return Result.Fail(ErrorResults.InternalServerError(ex));
        }
    }

    private static bool GroupAlreadyHavePricingPlan(PricingPlan pricingPlan, List<PricingPlan> existingPricingPlans)
    {
        return existingPricingPlans
            .Any(entity => string.Equals(
                entity.Group.Name,
                pricingPlan.Group.Name,
                StringComparison.CurrentCultureIgnoreCase)
            );
    }
}
