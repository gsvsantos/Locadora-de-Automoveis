using AutoMapper;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Groups.Commands.Update;

public class UpdateGroupRequestHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IRepositoryGroup repositoryGroup,
    IDistributedCache cache,
    IValidator<Group> validator,
    ILogger<UpdateGroupRequestHandler> logger
) : IRequestHandler<UpdateGroupRequest, Result<UpdateGroupResponse>>
{
    public async Task<Result<UpdateGroupResponse>> Handle(
        UpdateGroupRequest request, CancellationToken cancellationToken)
    {
        Group? selectedGroup = await repositoryGroup.GetByIdAsync(request.Id);

        if (selectedGroup is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(request.Id));
        }

        try
        {
            Group updatedGroup = mapper.Map<Group>(request);

            ValidationResult validationResult = await validator.ValidateAsync(updatedGroup, cancellationToken);

            if (!validationResult.IsValid)
            {
                List<string> errors = validationResult.Errors
                    .Select(failure => failure.ErrorMessage)
                    .ToList();

                return Result.Fail(ErrorResults.BadRequestError(errors));
            }

            List<Group> existingGroups = await repositoryGroup.GetAllAsync();

            if (DuplicateName(updatedGroup, existingGroups))
            {
                return Result.Fail(GroupErrorResults.DuplicateNameError(request.Name));
            }

            await repositoryGroup.UpdateAsync(request.Id, updatedGroup);

            await unitOfWork.CommitAsync();

            await cache.SetStringAsync("groups:master-version", Guid.NewGuid().ToString(), cancellationToken);

            return Result.Ok(new UpdateGroupResponse(request.Id));
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

    private static bool DuplicateName(Group group, List<Group> existingGroups)
    {
        return existingGroups
            .Any(entity =>
            entity.Id != group.Id &&
            string.Equals(
                entity.Name,
                group.Name,
                StringComparison.CurrentCultureIgnoreCase)
            );
    }
}
