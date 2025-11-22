using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Groups;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Groups.Commands.GetAll;

public class GetAllGroupRequestHandler(
    IRepositoryGroup repositoryGroup,
    ILogger<GetAllGroupRequestHandler> logger
) : IRequestHandler<GetAllGroupRequest, Result<GetAllGroupResponse>>
{
    public async Task<Result<GetAllGroupResponse>> Handle(
        GetAllGroupRequest request, CancellationToken cancellationToken)
    {
        try
        {
            List<Group> groups =
                request.Quantity.HasValue && request.Quantity.Value > 0 ?
                await repositoryGroup.GetAllAsync(request.Quantity.Value) :
                await repositoryGroup.GetAllAsync();

            GetAllGroupResponse response = new(
                groups.Count,
                groups.Select(group => new GroupDto(
                    group.Id,
                    group.Name
                )).ToImmutableList()
            );

            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred during the request. \n{@Request}.", request
            );

            return Result.Fail(ErrorResults.InternalServerError(ex));
        }
    }
}
