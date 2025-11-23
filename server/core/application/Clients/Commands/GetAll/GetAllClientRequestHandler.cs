using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Clients;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Clients.Commands.GetAll;

public class GetAllClientRequestHandler(
    IRepositoryClient repositoryClient,
    ILogger<GetAllClientRequestHandler> logger
) : IRequestHandler<GetAllClientRequest, Result<GetAllClientResponse>>
{
    public async Task<Result<GetAllClientResponse>> Handle(
        GetAllClientRequest request, CancellationToken cancellationToken)
    {
        try
        {
            List<Client> clients =
                request.Quantity.HasValue && request.Quantity.Value > 0
                ? await repositoryClient.GetAllAsync(request.Quantity.Value)
                : await repositoryClient.GetAllAsync();

            GetAllClientResponse response = new(
                clients.Count,
                clients.Select(client => new ClientDto(
                    client.Id,
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
