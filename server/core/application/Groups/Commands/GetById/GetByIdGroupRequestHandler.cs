using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Groups;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Groups.Commands.GetById;

public class GetByIdGroupRequestHandler(
    IMapper mapper,
    IRepositoryGroup repositoryGroup,
    ILogger<GetByIdGroupRequestHandler> logger
) : IRequestHandler<GetByIdGroupRequest, Result<GetByIdGroupResponse>>
{
    public async Task<Result<GetByIdGroupResponse>> Handle(
        GetByIdGroupRequest request, CancellationToken cancellationToken)
    {
        try
        {
            Group? selectedGroup = await repositoryGroup.GetByIdAsync(request.Id);

            if (selectedGroup == null)
            {
                return Result.Fail(ErrorResults.NotFoundError(request.Id));
            }

            GetByIdGroupResponse response = mapper.Map<GetByIdGroupResponse>(selectedGroup);

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
