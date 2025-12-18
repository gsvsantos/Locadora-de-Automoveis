using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Clients;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Clients.Commands.GetClientProfile;

public class GetClientProfileRequestHandler(
    IUserContext userContext,
    IRepositoryClient repositoryClient,
    IMapper mapper
) : IRequestHandler<GetClientProfileRequest, Result<GetClientProfileResponse>>
{
    public async Task<Result<GetClientProfileResponse>> Handle(
        GetClientProfileRequest request, CancellationToken cancellationToken)
    {
        Guid userId = userContext.GetUserId();

        Client? client = await repositoryClient.GetByUserIdAsync(userId);

        if (client is null)
        {
            return Result.Fail(ErrorResults.NotFoundError("Client profile not found."));
        }

        ClientProfileDto clientDto = mapper.Map<ClientProfileDto>(client);

        return Result.Ok(new GetClientProfileResponse(clientDto));
    }
}
