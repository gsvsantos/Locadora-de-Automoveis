using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Account.Commands.GetProfile;

public class GetDetailsRequestHandler(
    IMapper mapper,
    IUserContext userContext,
    IUnitOfWork unitOfWork,
    IRepositoryClient repositoryClient,
    ILogger<GetDetailsRequestHandler> logger
) : IRequestHandler<GetDetailsRequest, Result<GetDetailsResponse>>
{
    public async Task<Result<GetDetailsResponse>> Handle(
        GetDetailsRequest request, CancellationToken cancellationToken)
    {
        try
        {
            Guid loginUserId = userContext.GetUserId();

            Client? client = await repositoryClient.GetGlobalByLoginUserIdAsync(loginUserId);

            if (client is null)
            {
                return Result.Fail(ErrorResults.NotFoundError("Client profile not found."));
            }

            ClientProfileDto clientDto = mapper.Map<ClientProfileDto>(client);

            return Result.Ok(new GetDetailsResponse(clientDto));
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
