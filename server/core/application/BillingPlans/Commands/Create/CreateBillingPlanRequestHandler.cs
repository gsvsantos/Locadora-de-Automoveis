using AutoMapper;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.BillingPlans;
using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.BillingPlans.Commands.Create;

public class CreateBillingPlanRequestHandler(
    UserManager<User> userManager,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IRepositoryBillingPlan repositoryBillingPlan,
    IRepositoryGroup repositoryGroup,
    ITenantProvider tenantProvider,
    IUserContext userContext,
    IValidator<BillingPlan> validator,
    ILogger<CreateBillingPlanRequestHandler> logger
) : IRequestHandler<CreateBillingPlanRequest, Result<CreateBillingPlanResponse>>
{
    public async Task<Result<CreateBillingPlanResponse>> Handle(
        CreateBillingPlanRequest request, CancellationToken cancellationToken)
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

        BillingPlan BillingPlan = mapper.Map<BillingPlan>((request, selectedGroup));
        BillingPlan.AssociateGroup(selectedGroup);

        try
        {
            ValidationResult validationResult = await validator.ValidateAsync(BillingPlan, cancellationToken);

            if (!validationResult.IsValid)
            {
                List<string> errors = validationResult.Errors
                    .Select(failure => failure.ErrorMessage)
                    .ToList();

                return Result.Fail(ErrorResults.BadRequestError(errors));
            }

            List<BillingPlan> existingBillingPlans = await repositoryBillingPlan.GetAllAsync();

            if (GroupAlreadyHaveBillingPlan(BillingPlan, existingBillingPlans))
            {
                return Result.Fail(BillingPlanErrorResults.GroupAlreadyHaveBillingPlanError(selectedGroup.Name));
            }

            BillingPlan.AssociateTenant(tenantProvider.GetTenantId());

            BillingPlan.AssociateUser(user);

            await repositoryBillingPlan.AddAsync(BillingPlan);

            await unitOfWork.CommitAsync();

            return Result.Ok(new CreateBillingPlanResponse(BillingPlan.Id));
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

    private static bool GroupAlreadyHaveBillingPlan(BillingPlan BillingPlan, List<BillingPlan> existingBillingPlans)
    {
        return existingBillingPlans
            .Any(entity => string.Equals(
                entity.Group.Name,
                BillingPlan.Group.Name,
                StringComparison.CurrentCultureIgnoreCase)
            );
    }
}
