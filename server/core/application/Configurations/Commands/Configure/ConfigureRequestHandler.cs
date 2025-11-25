using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Configurations;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Configurations.Commands.Configure;

public class ConfigureRequestHandler(
    UserManager<User> userManager,
    IUnitOfWork unitOfWork,
    IRepositoryConfiguration repositoryConfiguration,
    ITenantProvider tenantProvider,
    IUserContext userContext,
    IValidator<Configuration> validator,
    ILogger<ConfigureRequestHandler> logger
) : IRequestHandler<ConfigureRequest, Result<ConfigureResponse>>
{
    public async Task<Result<ConfigureResponse>> Handle(
        ConfigureRequest request, CancellationToken cancellationToken)
    {
        User? user = await userManager.FindByIdAsync(userContext.GetUserId().ToString());

        if (user is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(userContext.GetUserId()));
        }

        Configuration tempConfig = new(
            request.GasolinePrice,
            request.GasPrice,
            request.DieselPrice,
            request.AlcoholPrice
        );

        try
        {
            ValidationResult validationResult = await validator.ValidateAsync(tempConfig, cancellationToken);

            if (!validationResult.IsValid)
            {
                List<string> errors = validationResult.Errors
                    .Select(failure => failure.ErrorMessage)
                    .ToList();

                return Result.Fail(ErrorResults.BadRequestError(errors));
            }

            Guid tenantId = tenantProvider.GetTenantId();
            Configuration? configutarion = await repositoryConfiguration.GetByTenantId(tenantId);

            if (configutarion is null)
            {
                configutarion = tempConfig;

                configutarion.AssociateTenant(tenantId);
                configutarion.AssociateUser(user);

                await repositoryConfiguration.AddAsync(configutarion);
            }
            else
            {
                configutarion.AssociateUser(user);

                await repositoryConfiguration.UpdateAsync(configutarion.Id, tempConfig);
            }

            await unitOfWork.CommitAsync();

            return Result.Ok(new ConfigureResponse(configutarion.Id));
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
}
