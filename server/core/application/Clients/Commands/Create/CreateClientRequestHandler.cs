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

namespace LocadoraDeAutomoveis.Application.Clients.Commands.Create;

public class CreateClientRequestHandler(
    UserManager<User> userManager,
    IUnitOfWork unitOfWork,
    IRepositoryClient repositoryClient,
    ITenantProvider tenantProvider,
    IUserContext userContext,
    IValidator<Client> validator,
    ILogger<CreateClientRequestHandler> logger
) : IRequestHandler<CreateClientRequest, Result<CreateClientResponse>>
{
    public async Task<Result<CreateClientResponse>> Handle(
        CreateClientRequest request, CancellationToken cancellationToken)
    {
        User? user = await userManager.FindByIdAsync(userContext.GetUserId().ToString());

        if (user is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(userContext.GetUserId()));
        }

        Client client = new(
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
        );

        try
        {
            ValidationResult validationResult = await validator.ValidateAsync(client, cancellationToken);

            if (!validationResult.IsValid)
            {
                List<string> errors = validationResult.Errors
                    .Select(failure => failure.ErrorMessage)
                    .ToList();

                return Result.Fail(ErrorResults.BadRequestError(errors));
            }

            List<Client> existingClients = await repositoryClient.GetAllAsync();

            if (DocumentAlreadyRegistred(client, existingClients))
            {
                return Result.Fail(ClientErrorResults.DocumentAlreadyRegistredError(request.Document!));
            }

            client.AssociateTenant(tenantProvider.GetTenantId());

            client.AssociateUser(user);

            await repositoryClient.AddAsync(client);

            await unitOfWork.CommitAsync();

            return Result.Ok(new CreateClientResponse(client.Id));
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync();

            logger.LogError(
                ex,
                "An error occurred during registration. \n{@Request}.", request
            );

            return Result.Fail(ErrorResults.InternalServerError(ex));
        }
    }

    private static bool DocumentAlreadyRegistred(Client client, List<Client> existingClients)
    {
        return existingClients
            .Any(entity => string.Equals(
                entity.Document,
                client.Document,
                StringComparison.CurrentCultureIgnoreCase)
            );
    }
}
