using AutoMapper;
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
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Drivers.Commands.Create;

public class CreateDriverRequestHandler(
    UserManager<User> userManager,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IRepositoryDriver repositoryDriver,
    IRepositoryClient repositoryClient,
    IDistributedCache cache,
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

        bool isNewDriver = false;
        Driver? driver = await repositoryDriver.GetByDocumentAsync(request.Document);

        if (driver is not null)
        {
            if (driver.IsActive)
            {
                return Result.Fail(DriverErrorResults.DocumentAlreadyRegistredError(request.Document));
            }

            mapper.Map(request, driver);

            driver.Activate();
        }
        else
        {
            driver = mapper.Map<Driver>(request);

            isNewDriver = true;
        }

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

            if (selectedClient.Type == EClientType.Business)
            {
                Client? newCLient = null!;

                if (request.IndividualClientId.HasValue)
                {
                    newCLient = await repositoryClient.GetByIdAsync(request.IndividualClientId.Value);

                    if (newCLient is null)
                    {
                        return Result.Fail(ErrorResults.NotFoundError(request.IndividualClientId.Value));
                    }
                }
                else
                {
                    bool documentExists = await repositoryClient.ExistsByDocumentAsync(request.Document);

                    if (documentExists)
                    {
                        Client? existingClient = await repositoryClient.GetByTenantAndDocumentAsync(tenantProvider.GetTenantId(), request.Document);
                        if (existingClient != null)
                        {
                            newCLient = existingClient;
                        }
                        else
                        {
                            return Result.Fail(ClientErrorResults.DocumentAlreadyRegistredError(request.Document));
                        }
                    }
                    else
                    {
                        newCLient = CreateNewClient(request, user, selectedClient);

                        await repositoryClient.AddAsync(newCLient);
                    }
                }

                driver.AssociateClient(newCLient);
            }
            else
            {
                selectedClient.SetLicenseNumber(request.LicenseNumber);
                selectedClient.SetLicenseValidity(request.LicenseValidity);

                driver.AssociateClient(selectedClient);
            }

            driver.AssociateTenant(tenantProvider.GetTenantId());

            driver.AssociateUser(user);

            if (isNewDriver)
            {
                await repositoryDriver.AddAsync(driver);
            }
            else
            {
                await repositoryDriver.UpdateAsync(driver.Id, driver);
            }

            await unitOfWork.CommitAsync();

            await cache.SetStringAsync("clients:master-version", Guid.NewGuid().ToString(), cancellationToken);
            await cache.SetStringAsync("drivers:master-version", Guid.NewGuid().ToString(), cancellationToken);

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

    private Client CreateNewClient(CreateDriverRequest request, User user, Client selectedClient)
    {
        Address addressCopy = selectedClient.Address! with { };
        Client newCLient = mapper.Map<Client>((request, addressCopy));
        newCLient.DefineType(EClientType.Individual);
        newCLient.AssociateTenant(tenantProvider.GetTenantId());
        newCLient.AssociateUser(user);
        newCLient.AssociateJuristicClient(selectedClient);
        newCLient.SetLicenseNumber(request.LicenseNumber);
        newCLient.SetLicenseValidity(request.LicenseValidity);
        return newCLient;
    }
}
