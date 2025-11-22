using FluentResults;
using LocadoraDeAutomoveis.Application.Groups.Commands.GetAll;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Groups;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Groups.Commands.GetById;

public class GetByIdGroupRequestHandler(
    IRepositoryGroup repositoryGroup,
    ILogger<GetByIdGroupRequestHandler> logger
) : IRequestHandler<GetByIdGroupRequest, Result<GetByIdGroupResponse>>
{
    public async Task<Result<GetByIdGroupResponse>> Handle(
        GetByIdGroupRequest request, CancellationToken cancellationToken)
    {
        try
        {
            Group? group = await repositoryGroup.GetByIdAsync(request.Id);

            if (group == null)
            {
                return Result.Fail(ErrorResults.NotFoundError(request.Id));
            }

            GetByIdGroupResponse response = new(
                 new GroupDto(
                    group.Id,
                    group.Name
                )
            );

            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error retrieving group by ID {GroupId}", request.Id);

            return Result.Fail(new Error("An error occurred while retrieving the group."));
        }
    }
}
