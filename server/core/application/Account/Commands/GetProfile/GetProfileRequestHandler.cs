using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Account.Commands.GetClientProfile;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Account.Commands.GetProfile;

public class GetProfileRequestHandler(
    IMapper mapper,
    IUserContext userContext,
    IUnitOfWork unitOfWork,
    IRepositoryClient repositoryClient,
    ILogger<GetProfileRequestHandler> logger
) : IRequestHandler<GetProfileRequest, Result<GetProfileResponse>>
{
    public async Task<Result<GetProfileResponse>> Handle(
        GetProfileRequest request, CancellationToken cancellationToken)
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

            return Result.Ok(new GetProfileResponse(clientDto));
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
