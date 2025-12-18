using AutoMapper;
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
    IMapper mapper,
    IRepositoryClient repositoryClient,
    ITenantProvider tenantProvider,
    IUserContext userContext,
    IAuthEmailService emailService,
    IValidator<Client> validator,
    ILogger<CreateClientRequestHandler> logger
) : IRequestHandler<CreateClientRequest, Result<CreateClientResponse>>
{
    public async Task<Result<CreateClientResponse>> Handle(
        CreateClientRequest request, CancellationToken cancellationToken)
    {
        User? currentUser = await userManager.FindByIdAsync(userContext.GetUserId().ToString());

        if (currentUser is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(userContext.GetUserId()));
        }

        User userLogin = mapper.Map<User>(request);

        try
        {
            IdentityResult identityResult = await userManager.CreateAsync(userLogin);

            if (!identityResult.Succeeded)
            {
                IEnumerable<string> errors = identityResult
                    .Errors
                    .Select(failure => failure.Description)
                    .ToList();

                return Result.Fail(ErrorResults.BadRequestError(errors));
            }

            await userManager.AddToRoleAsync(userLogin, "Client");

            userLogin.AssociateTenant(tenantProvider.GetTenantId());

            await userManager.UpdateAsync(userLogin);

            Address address = mapper.Map<Address>(request);
            Client client = mapper.Map<Client>((request, address));

            client.DefineType(request.Type);

            ValidationResult validationResult = await validator.ValidateAsync(
                client,
                options => options.IncludeRuleSets("default", "Complete"),
                cancellationToken
            );

            if (!validationResult.IsValid)
            {
                await userManager.DeleteAsync(userLogin);

                List<string> errors = validationResult.Errors
                    .Select(failure => failure.ErrorMessage)
                    .ToList();

                return Result.Fail(ErrorResults.BadRequestError(errors));
            }

            bool documentExists = await repositoryClient.ExistsByDocumentAsync(client.Document!);

            if (documentExists)
            {
                await userManager.DeleteAsync(userLogin);
                return Result.Fail(ClientErrorResults.DocumentAlreadyRegistredError(client.Document!));
            }
            client.AssociateLoginUser(userLogin);
            client.AssociateUser(currentUser);
            client.AssociateTenant(tenantProvider.GetTenantId());

            await repositoryClient.AddAsync(client);

            await unitOfWork.CommitAsync();

            string token = await userManager.GeneratePasswordResetTokenAsync(userLogin);

            try
            {
                await emailService.ScheduleClientInvitation(userLogin.Email!, userLogin.FullName, token);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to schedule invitation email for user {UseriD}", userLogin.Id);
            }

            return Result.Ok(new CreateClientResponse(client.Id));
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync();

            if (userLogin.Id != Guid.Empty)
            {
                await userManager.DeleteAsync(userLogin);
            }

            logger.LogError(
                ex,
                "An error occurred during registration. \n{@Request}.", request
            );

            return Result.Fail(ErrorResults.InternalServerError(ex));
        }
    }
}
