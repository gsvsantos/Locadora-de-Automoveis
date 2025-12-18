using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Groups.Commands.GetAll;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Groups;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Groups.Commands.GetAllDistinct;

public class GetAllDistinctGroupsRequestHandler(
    IRepositoryGroup repositoryGroup,
    IMapper mapper,
    ILogger<GetAllDistinctGroupsRequestHandler> logger
) : IRequestHandler<GetAllDistinctGroupsRequest, Result<List<GroupDto>>>
{
    public async Task<Result<List<GroupDto>>> Handle(
        GetAllDistinctGroupsRequest request, CancellationToken cancellationToken)
    {
        try
        {
            List<Group> groups = await repositoryGroup.GetAllDistinct();

            List<Group> distinctGroups = groups
                .GroupBy(g => g.Name.ToUpper().Trim())
                .Select(g => g.First())
                .ToList();

            List<GroupDto> dtos = mapper.Map<List<GroupDto>>(distinctGroups);

            return Result.Ok(dtos);
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