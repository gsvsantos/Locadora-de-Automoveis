using AutoMapper;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Drivers;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Clients.Commands.Update;

public class UpdateClientRequestHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IRepositoryClient repositoryClient,
    IRepositoryDriver repositoryDriver,
    IDistributedCache cache,
    IValidator<Client> validator,
    ILogger<UpdateClientRequestHandler> logger
) : IRequestHandler<UpdateClientRequest, Result<UpdateClientResponse>>
{
    public async Task<Result<UpdateClientResponse>> Handle(
        UpdateClientRequest request, CancellationToken cancellationToken)
    {
        Client? selectedClient = await repositoryClient.GetByIdAsync(request.Id);

        if (selectedClient is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(request.Id));
        }

        Address address = mapper.Map<Address>(request);
        Client updatedClient = mapper.Map<Client>((request, address));

        updatedClient.DefineType(request.Type);

        try
        {
            ValidationResult validationResult = await validator.ValidateAsync(updatedClient, cancellationToken);

            if (!validationResult.IsValid)
            {
                List<string> errors = validationResult.Errors
                    .Select(failure => failure.ErrorMessage)
                    .ToList();

                return Result.Fail(ErrorResults.BadRequestError(errors));
            }

            List<Client> existingClients = await repositoryClient.GetAllAsync();

            if (DocumentAlreadyRegistred(updatedClient, existingClients))
            {
                return Result.Fail(ClientErrorResults.DocumentAlreadyRegistredError(request.Document));
            }

            bool driverFound = await repositoryDriver.HasDriversByClient(selectedClient.Id);

            if (driverFound)
            {
                Driver? driver = await repositoryDriver.GetDriverByClientId(selectedClient.Id);

                driver!.FullName = request.FullName;
            }

            await repositoryClient.UpdateAsync(selectedClient.Id, updatedClient);

            await unitOfWork.CommitAsync();

            await cache.SetStringAsync("clients:master-version", Guid.NewGuid().ToString(), cancellationToken);

            return Result.Ok(new UpdateClientResponse(selectedClient.Id));
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

    private static bool DocumentAlreadyRegistred(Client client, List<Client> existingClients)
    {
        return existingClients
            .Any(entity =>
            entity.Id != client.Id &&
            string.Equals(
                entity.Document,
                client.Document,
                StringComparison.CurrentCultureIgnoreCase)
            );
    }
}
