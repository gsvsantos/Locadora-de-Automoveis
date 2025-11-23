using FluentResults;
using LocadoraDeAutomoveis.Application.Clients.Commands.GetAll;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Clients;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Clients.Commands.GetById;

public class GetByIdClientRequestHandler(
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

            GetByIdClientResponse response = new(
                new ClientDto(
                    request.Id,
                    client.FullName,
                    client.Email,
                    client.PhoneNumber,
                    client.IsJuridical,
                    client.State,
                    client.City,
                    client.Neighborhood,
                    client.Street,
                    client.Number,
                    client.Document,
                    client.LicenseNumber
                )
            );

            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error retrieving ClientCPF by ID {ClientCPFId}", request.Id);

            return Result.Fail(new Error("An error occurred while retrieving the client."));
        }
    }
}
