using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Clients;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Clients.Commands.GetById;

public class GetByIdClientRequestHandler(
    IMapper mapper,
    IRepositoryClient repositoryClient,
    ILogger<GetByIdClientRequestHandler> logger
) : IRequestHandler<GetByIdClientRequest, Result<GetByIdClientResponse>>
{
    public async Task<Result<GetByIdClientResponse>> Handle(
        GetByIdClientRequest request, CancellationToken cancellationToken)
    {
        try
        {
            Client? client = await repositoryClient.GetByIdAsync(request.Id);

            if (client is null)
            {
                return Result.Fail(ErrorResults.NotFoundError(request.Id));
            }

            GetByIdClientResponse response = mapper.Map<GetByIdClientResponse>(client);

            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error retrieving Client by ID {ClientId}", request.Id);

            return Result.Fail(new Error("An error occurred while retrieving the client."));
        }
    }
}
