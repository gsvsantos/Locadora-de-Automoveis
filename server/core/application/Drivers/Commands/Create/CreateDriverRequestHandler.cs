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
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Drivers.Commands.Create;

public class CreateDriverRequestHandler(
    UserManager<User> userManager,
    IUnitOfWork unitOfWork,
    IMapper mapper,
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

        Driver driver = mapper.Map<Driver>(request);

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

            if (DriverDocumentAlreadyRegistred(request.Document, existingDrivers))
            {
                return Result.Fail(DriverErrorResults.DocumentAlreadyRegistredError(request.Document));
            }

            if (selectedClient.Type == EClientType.Business)
            {
                Client? newCLient = null!;

                bool businessClientHasIndividuals = await repositoryClient.BusinessClientHasIndividuals(selectedClient.Id);

                if (businessClientHasIndividuals)
                {
                    if (!request.IndividualClientId.HasValue)
                    {
                        return Result.Fail(DriverErrorResults.IndividualClientIdError());
                    }
                    else
                    {
                        newCLient = await repositoryClient.GetByIdAsync(request.IndividualClientId.Value);

                        if (newCLient is null)
                        {
                            return Result.Fail(ErrorResults.NotFoundError(request.IndividualClientId.Value));
                        }
                    }
                }
                else
                {
                    if (ClientDocumentAlreadyRegistred(request.Document, existingClients))
                    {
                        return Result.Fail(ClientErrorResults.DocumentAlreadyRegistredError(request.Document));
                    }

                    newCLient = CreateNewClient(request, user, selectedClient);

                    await repositoryClient.AddAsync(newCLient);
                }

                driver.AssociateClient(newCLient);
            }
            else
            {
                selectedClient.SetLicenseNumber(request.LicenseNumber);

                driver.AssociateClient(selectedClient);
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

    private Client CreateNewClient(CreateDriverRequest request, User user, Client selectedClient)
    {
        Address addressCopy = selectedClient.Address with { };
        Client newCLient = mapper.Map<Client>((request, addressCopy));
        newCLient.DefineType(EClientType.Individual);
        newCLient.AssociateTenant(tenantProvider.GetTenantId());
        newCLient.AssociateUser(user);
        newCLient.AssociateJuristicClient(selectedClient);
        newCLient.SetLicenseNumber(request.LicenseNumber);
        return newCLient;
    }

    private static bool ClientDocumentAlreadyRegistred(string document, List<Client> existingClients)
    {
        return existingClients.Any(entity =>
            string.Equals(entity.Document, document, StringComparison.CurrentCultureIgnoreCase));
    }

    private static bool DriverDocumentAlreadyRegistred(string document, List<Driver> existingDrivers)
    {
        return existingDrivers.Any(entity =>
            string.Equals(entity.Document, document, StringComparison.CurrentCultureIgnoreCase));
    }
}
