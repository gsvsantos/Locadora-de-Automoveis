using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using LocadoraDeAutomoveis.Application.Clients;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Drivers;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Drivers.Commands.Create;

public class CreateDriverRequestHandler(
    UserManager<User> userManager,
    IUnitOfWork unitOfWork,
    IRepositoryDriver repositoryDriver,
    IRepositoryClient repositoryClient,
    ITenantProvider tenantProvider,
    IUserContext userContext,
    IValidator<Driver> validator,
    ILogger<CreateDriverRequestHandler> logger
) : IRequestHandler<CreateDriverRequest, Result<CreateDriverResponse>>
{
    public async Task<Result<CreateDriverResponse>> Handle(
        CreateDriverRequest request, CancellationToken cancellationToken)
    {
        User? user = await userManager.FindByIdAsync(userContext.GetUserId().ToString());

        if (user is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(userContext.GetUserId()));
        }

        Client? selectedClient = await repositoryClient.GetByIdAsync(request.ClientId);

        if (selectedClient is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(request.ClientId));
        }

        Driver driver = new(
            request.FullName,
            request.Email,
            request.PhoneNumber,
            request.Document,
            request.LicenseNumber,
            request.LicenseValidity
        );

        try
        {
            ValidationResult validationResult = await validator.ValidateAsync(driver, cancellationToken);

            if (!validationResult.IsValid)
            {
                List<string> errors = validationResult.Errors
                    .Select(failure => failure.ErrorMessage)
                    .ToList();

                return Result.Fail(ErrorResults.BadRequestError(errors));
            }

            List<Client> existingClients = await repositoryClient.GetAllAsync();

            List<Driver> existingDrivers = await repositoryDriver.GetAllAsync();

            if (selectedClient.IsJuridical && DocumentAlreadyRegistred(selectedClient, existingClients))
            {
                return Result.Fail(ClientErrorResults.DocumentAlreadyRegistredError(request.Document));
            }

            if (selectedClient.IsJuridical)
            {
                Client newCLient = new(
                    request.FullName,
                    request.Email,
                    request.PhoneNumber,
                    selectedClient.State,
                    selectedClient.City,
                    selectedClient.Neighborhood,
                    selectedClient.Street,
                    selectedClient.Number,
                    request.Document
                );
                newCLient.MarkAsPhysical();

                newCLient.AssociateTenant(tenantProvider.GetTenantId());

                newCLient.AssociateUser(user);

                driver.AssociateClientCPF(newCLient);

                driver.AssociateClientCNPJ(newCLient);

                await repositoryClient.AddAsync(newCLient);
            }
            else
            {
                driver.AssociateClientCPF(selectedClient);
            }

            driver.AssociateTenant(tenantProvider.GetTenantId());

            driver.AssociateUser(user);

            await repositoryDriver.AddAsync(driver);

            await unitOfWork.CommitAsync();

            return Result.Ok(new CreateDriverResponse(driver.Id));
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
            .Any(entity =>
            entity.Id != client.Id &&
            string.Equals(
                entity.Document,
                client.Document,
                StringComparison.CurrentCultureIgnoreCase)
            );
    }
}
