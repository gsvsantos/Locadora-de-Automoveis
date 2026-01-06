using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.BillingPlans;
using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Domain.Vehicles;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Groups.Commands.Delete;

public class DeleteGroupRequestHandler(
    IUnitOfWork unitOfWork,
    IRepositoryGroup repositoryGroup,
    IRepositoryVehicle repositoryVehicle,
    IRepositoryBillingPlan repositoryBillingPlan,
    IDistributedCache cache,
    ILogger<DeleteGroupRequestHandler> logger
) : IRequestHandler<DeleteGroupRequest, Result<DeleteGroupResponse>>
{
    public async Task<Result<DeleteGroupResponse>> Handle(
        DeleteGroupRequest request, CancellationToken cancellationToken)
    {
        Group? selectedGroup = await repositoryGroup.GetByIdAsync(request.Id);

        if (selectedGroup is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(request.Id));
        }

        bool hasVehicles = await repositoryVehicle.ExistsByGroupId(request.Id);
        bool hasBillingPlans = await repositoryBillingPlan.ExistsByGroupId(request.Id);

        bool isInUse = hasVehicles || hasBillingPlans;

        try
        {
            if (isInUse)
            {
                selectedGroup.Deactivate();

                await repositoryGroup.UpdateAsync(selectedGroup.Id, selectedGroup);

                logger.LogInformation(
                    "Group {GroupId} was deactivated instead of deleted because it is in use.",
                    request.Id
                );
            }
            else
            {
                await repositoryGroup.DeleteAsync(request.Id);
            }

            await unitOfWork.CommitAsync();

            await cache.SetStringAsync("groups:master-version", Guid.NewGuid().ToString(), cancellationToken);

            return Result.Ok(new DeleteGroupResponse());
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
