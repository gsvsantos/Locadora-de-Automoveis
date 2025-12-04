using AutoMapper;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.RentalExtras;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.RentalExtras.Commands.Create;

public class CreateRentalExtraRequestHandler(
    UserManager<User> userManager,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IRepositoryRentalExtra repositoryRentalExtra,
    ITenantProvider tenantProvider,
    IUserContext userContext,
    IValidator<RentalExtra> validator,
    ILogger<CreateRentalExtraRequestHandler> logger
) : IRequestHandler<CreateRentalExtraRequest, Result<CreateRentalExtraResponse>>
{
    public async Task<Result<CreateRentalExtraResponse>> Handle(
        CreateRentalExtraRequest request, CancellationToken cancellationToken)
    {
        User? user = await userManager.FindByIdAsync(userContext.GetUserId().ToString());

        if (user is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(userContext.GetUserId()));
        }

        RentalExtra rentalExtra = mapper.Map<RentalExtra>(request);
        rentalExtra.DefineType(request.Type);

        try
        {
            ValidationResult validationResult = await validator.ValidateAsync(rentalExtra, cancellationToken);

            if (!validationResult.IsValid)
            {
                List<string> errors = validationResult.Errors
                    .Select(failure => failure.ErrorMessage)
                    .ToList();

                return Result.Fail(ErrorResults.BadRequestError(errors));
            }

            List<RentalExtra> existingRentalExtras = await repositoryRentalExtra.GetAllAsync();

            if (DuplicatedName(rentalExtra, existingRentalExtras))
            {
                return Result.Fail(RentalExtraErrorResults.DuplicateNameError(request.Name));
            }

            rentalExtra.AssociateTenant(tenantProvider.GetTenantId());

            rentalExtra.AssociateUser(user);

            if (request.IsFixed)
            {
                rentalExtra.MarkAsFixed();
            }
            else
            {
                rentalExtra.MarkAsDaily();
            }

            await repositoryRentalExtra.AddAsync(rentalExtra);

            await unitOfWork.CommitAsync();

            return Result.Ok(new CreateRentalExtraResponse(rentalExtra.Id));
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

    private static bool DuplicatedName(RentalExtra rentalExtra, List<RentalExtra> existingRentalExtras)
    {
        return existingRentalExtras
            .Any(entity => string.Equals(
                entity.Name,
                rentalExtra.Name,
                StringComparison.CurrentCultureIgnoreCase)
            );
    }
}
