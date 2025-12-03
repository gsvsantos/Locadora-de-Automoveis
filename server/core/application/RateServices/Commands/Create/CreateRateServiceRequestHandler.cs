using AutoMapper;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.RateServices;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.RateServices.Commands.Create;

public class CreateRateServiceRequestHandler(
    UserManager<User> userManager,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IRepositoryRateService repositoryRateService,
    ITenantProvider tenantProvider,
    IUserContext userContext,
    IValidator<RateService> validator,
    ILogger<CreateRateServiceRequestHandler> logger
) : IRequestHandler<CreateRateServiceRequest, Result<CreateRateServiceResponse>>
{
    public async Task<Result<CreateRateServiceResponse>> Handle(
        CreateRateServiceRequest request, CancellationToken cancellationToken)
    {
        User? user = await userManager.FindByIdAsync(userContext.GetUserId().ToString());

        if (user is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(userContext.GetUserId()));
        }

        RateService rateService = mapper.Map<RateService>(request);
        rateService.DefineRateType(request.RateType);

        try
        {
            ValidationResult validationResult = await validator.ValidateAsync(rateService, cancellationToken);

            if (!validationResult.IsValid)
            {
                List<string> errors = validationResult.Errors
                    .Select(failure => failure.ErrorMessage)
                    .ToList();

                return Result.Fail(ErrorResults.BadRequestError(errors));
            }

            List<RateService> existingRateServices = await repositoryRateService.GetAllAsync();

            if (DuplicatedName(rateService, existingRateServices))
            {
                return Result.Fail(RateServiceErrorResults.DuplicateNameError(request.Name));
            }

            rateService.AssociateTenant(tenantProvider.GetTenantId());

            rateService.AssociateUser(user);

            if (request.IsFixed)
            {
                rateService.MarkAsFixed();
            }
            else
            {
                rateService.MarkAsDaily();
            }

            await repositoryRateService.AddAsync(rateService);

            await unitOfWork.CommitAsync();

            return Result.Ok(new CreateRateServiceResponse(rateService.Id));
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

    private static bool DuplicatedName(RateService rateService, List<RateService> existingRateServices)
    {
        return existingRateServices
            .Any(entity => string.Equals(
                entity.Name,
                rateService.Name,
                StringComparison.CurrentCultureIgnoreCase)
            );
    }
}
