using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Clients;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Clients.Commands.GetIndividuals;

public class GetIndividualsRequestHandler(
    IMapper mapper,
    IRepositoryClient repositoryClient,
    ILogger<GetIndividualsRequestHandler> logger
) : IRequestHandler<GetIndividualsRequest, Result<GetIndividualsResponse>>
{
    public async Task<Result<GetIndividualsResponse>> Handle(
        GetIndividualsRequest request, CancellationToken cancellationToken)
    {
        try
        {
            Client? selectedClient = await repositoryClient.GetByIdAsync(request.Id);

            if (selectedClient is null)
            {
                return Result.Fail(ErrorResults.NotFoundError(request.Id));
            }

            if (selectedClient.Type != EClientType.Business)
            {
                return Result.Fail(ClientErrorResults.ClientIsNotBusinessError(selectedClient.FullName));
            }

            List<Client> clients = await repositoryClient.GetIndividualClientsFromBusinessId(selectedClient.Id);

            GetIndividualsResponse response = mapper.Map<GetIndividualsResponse>(clients);

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
