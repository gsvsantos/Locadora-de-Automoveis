using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Groups;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Groups.Commands.GetAllDistinct;

public class GetAllDistinctGroupRequestHandler(
    IRepositoryGroup repositoryGroup,
    IMapper mapper,
    ILogger<GetAllDistinctGroupRequestHandler> logger
) : IRequestHandler<GetAllDistinctGroupRequest, Result<GetAllDistinctGroupResponse>>
{
    public async Task<Result<GetAllDistinctGroupResponse>> Handle(
        GetAllDistinctGroupRequest request, CancellationToken cancellationToken)
    {
        try
        {
            List<Group> groups = await repositoryGroup.GetAllDistinct();

            List<Group> distinctGroups = groups
                .GroupBy(g => g.Name.ToUpper().Trim())
                .Select(g => g.First())
                .ToList();

            GetAllDistinctGroupResponse response = mapper.Map<GetAllDistinctGroupResponse>(distinctGroups);

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