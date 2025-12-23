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
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Account.Commands.UpdateAccount;

public class UpdateAccountRequestHandler(
    IMapper mapper,
    IUserContext userContext,
    IUnitOfWork unitOfWork,
    IRepositoryClient repositoryClient,
    IRepositoryDriver repositoryDriver,
    IValidator<Client> validator,
    ILogger<UpdateAccountRequestHandler> logger
) : IRequestHandler<UpdateAccountRequest, Result<UpdateAccountResponse>>
{
    public async Task<Result<UpdateAccountResponse>> Handle(
        UpdateAccountRequest request, CancellationToken cancellationToken)
    {
        try
        {
            Guid loginUserId = userContext.GetUserId();

            Client? client = await repositoryClient.GetGlobalByLoginUserIdAsync(loginUserId);

            if (client is null)
            {
                return Result.Fail(ErrorResults.NotFoundError("Client profile not found."));
            }

            Address? address = mapper.Map<Address>(request);
            Client updatedClient = mapper.Map<Client>((request, address, client.Id));

            ValidationResult validationResult = await validator.ValidateAsync(updatedClient, cancellationToken);

            if (!validationResult.IsValid)
            {
                List<string> errors = validationResult.Errors
                    .Select(failure => failure.ErrorMessage)
                    .ToList();

                return Result.Fail(ErrorResults.BadRequestError(errors));
            }

            bool documentExists = await repositoryClient.ExistsByDocumentAsync(client.Document!, client.Id);

            if (documentExists)
            {
                return Result.Fail(ClientErrorResults.DocumentAlreadyRegistredError(client.Document!));
            }

            bool driverFound = await repositoryDriver.HasDriversByClient(client.Id);

            if (driverFound)
            {
                Driver? driver = await repositoryDriver.GetDriverByClientIdDistinctAsync(client.Id);

                driver!.FullName = request.FullName;
            }

            await repositoryClient.UpdateGlobalAsync(client.Id, updatedClient);

            await unitOfWork.CommitAsync();

            return Result.Ok(new UpdateAccountResponse());
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
}
