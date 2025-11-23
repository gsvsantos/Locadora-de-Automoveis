using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Clients.Commands.Update;

public class UpdateClientRequestHandler(
    UserManager<User> userManager,
    IUnitOfWork unitOfWork,
    IRepositoryClient repositoryClient,
    ITenantProvider tenantProvider,
    IUserContext userContext,
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

        Client updatedClient = new(
            request.FullName,
            request.Email,
            request.PhoneNumber,
            request.State,
            request.City,
            request.Neighborhood,
            request.Street,
            request.Number,
            request.Document,
            request.LicenseNumber
        )
        { Id = selectedClient.Id };

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
                return Result.Fail(ClientErrorResults.DocumentAlreadyRegistredError(request.Document!));
            }

            await repositoryClient.UpdateAsync(selectedClient.Id, updatedClient);

            await unitOfWork.CommitAsync();

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
            string.Equals(
                entity.Document,
                client.Document,
                StringComparison.CurrentCultureIgnoreCase)
            );
    }
}
