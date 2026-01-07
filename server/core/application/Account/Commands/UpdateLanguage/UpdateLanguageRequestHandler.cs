using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Account.Commands.UpdateLanguage;

public class UpdateLanguageRequestHandler(
    UserManager<User> userManager,
    IUserContext userContext,
    IUnitOfWork unitOfWork,
    IRepositoryClient repositoryClient,
    ILogger<UpdateLanguageRequestHandler> logger
) : IRequestHandler<UpdateLanguageRequest, Result<UpdateLanguageResponse>>
{
    public async Task<Result<UpdateLanguageResponse>> Handle(
        UpdateLanguageRequest request, CancellationToken cancellationToken)
    {
        try
        {
            Guid loginUserId = userContext.GetUserId();

            User? user = await userManager.FindByIdAsync(loginUserId.ToString());

            if (user is null)
            {
                return Result.Fail(ErrorResults.NotFoundError("User not found."));
            }

            IList<string> userRoles = await userManager.GetRolesAsync(user);

            if (userRoles.Contains("Client"))
            {

                Client? client = await repositoryClient.GetGlobalByLoginUserIdAsync(loginUserId);

                if (client is null)
                {
                    return Result.Fail(ErrorResults.NotFoundError("Client not found."));
                }

                client.SetPreferredLanguage(request.Language);

                await repositoryClient.UpdateAsync(client.Id, client);

                await unitOfWork.CommitAsync();
            }

            user.SetPreferredLanguage(request.Language);

            await userManager.UpdateAsync(user);

            IdentityResult identityResult = await userManager.UpdateAsync(user);

            if (!identityResult.Succeeded)
            {
                return Result.Fail(ErrorResults.BadRequestError("Failed to update user language."));
            }

            UpdateLanguageResponse response = new();

            return Result.Ok(response);
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
