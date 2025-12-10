using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Groups;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Groups.Commands.GetAll;

public class GetAllGroupRequestHandler(
    IMapper mapper,
    IRepositoryGroup repositoryGroup,
    ILogger<GetAllGroupRequestHandler> logger
) : IRequestHandler<GetAllGroupRequest, Result<GetAllGroupResponse>>
{
    public async Task<Result<GetAllGroupResponse>> Handle(
        GetAllGroupRequest request, CancellationToken cancellationToken)
    {
        try
        {
            List<Group> groups = [];
            bool quantityProvided = request.Quantity.HasValue && request.Quantity.Value > 0;
            bool inactiveProvided = request.IsActive.HasValue;

            if (quantityProvided && inactiveProvided)
            {
                int quantity = request.Quantity!.Value;
                bool isActive = request.IsActive!.Value;

                groups = await repositoryGroup.GetAllAsync(quantity, isActive);
            }
            else if (inactiveProvided)
            {
                bool isActive = request.IsActive!.Value;

                groups = await repositoryGroup.GetAllAsync(isActive);
            }
            else if (quantityProvided)
            {
                int quantity = request.Quantity!.Value;

                groups = await repositoryGroup.GetAllAsync(quantity);
            }
            else
            {
                groups = await repositoryGroup.GetAllAsync(true);
            }

            GetAllGroupResponse response = mapper.Map<GetAllGroupResponse>(groups);

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
