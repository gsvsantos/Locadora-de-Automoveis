using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Groups.Commands.Delete;

public class DeleteGroupRequestHandler(
    IUnitOfWork unitOfWork,
    IRepositoryGroup repositoryGroup,
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

        try
        {
            await repositoryGroup.DeleteAsync(request.Id);

            await unitOfWork.CommitAsync();

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
