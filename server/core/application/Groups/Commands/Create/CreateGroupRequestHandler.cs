using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Groups.Commands.Create;

public class CreateGroupRequestHandler(
    UserManager<User> userManager,
    IUnitOfWork unitOfWork,
    IRepositoryGroup repositoryGroup,
    ITenantProvider tenantProvider,
    IUserContext userContext,
    IValidator<Group> validator,
    ILogger<CreateGroupRequestHandler> logger
) : IRequestHandler<CreateGroupRequest, Result<CreateGroupResponse>>
{
    public async Task<Result<CreateGroupResponse>> Handle(
        CreateGroupRequest request, CancellationToken cancellationToken)
    {
        User? user = await userManager.FindByIdAsync(userContext.GetUserId().ToString());

        if (user is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(userContext.GetUserId()));
        }

        Group group = new(request.Name);

        try
        {
            ValidationResult validationResult = await validator.ValidateAsync(group, cancellationToken);

            if (!validationResult.IsValid)
            {
                List<string> errors = validationResult.Errors
                    .Select(failure => failure.ErrorMessage)
                    .ToList();

                return Result.Fail(ErrorResults.BadRequestError(errors));
            }

            List<Group> existingGroups = await repositoryGroup.GetAllAsync();

            if (DuplicateName(group, existingGroups))
            {
                return Result.Fail(GroupErrorResults.DuplicateNameError(request.Name));
            }

            group.AssociateTenant(tenantProvider.GetTenantId());

            group.AssociateUser(user);

            await repositoryGroup.AddAsync(group);

            await unitOfWork.CommitAsync();

            return Result.Ok(new CreateGroupResponse(group.Id));
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

    private static bool DuplicateName(Group group, List<Group> existingGroups)
    {
        return existingGroups
            .Any(entity => string.Equals(
                entity.Name,
                group.Name,
                StringComparison.CurrentCultureIgnoreCase)
            );
    }
}