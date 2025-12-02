using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Partners;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Partners.Commands.Create;

public class CreatePartnerRequestHandler(
    UserManager<User> userManager,
    IUnitOfWork unitOfWork,
    IRepositoryPartner repositoryPartner,
    ITenantProvider tenantProvider,
    IUserContext userContext,
    IValidator<Partner> validator,
    ILogger<CreatePartnerRequestHandler> logger
) : IRequestHandler<CreatePartnerRequest, Result<CreatePartnerResponse>>
{
    public async Task<Result<CreatePartnerResponse>> Handle(
        CreatePartnerRequest request, CancellationToken cancellationToken)
    {
        User? user = await userManager.FindByIdAsync(userContext.GetUserId().ToString());

        if (user is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(userContext.GetUserId()));
        }

        Partner partner = new(
            request.FullName
        );

        try
        {
            ValidationResult validationResult = await validator.ValidateAsync(partner, cancellationToken);

            if (!validationResult.IsValid)
            {
                List<string> errors = validationResult.Errors
                    .Select(failure => failure.ErrorMessage)
                    .ToList();

                return Result.Fail(ErrorResults.BadRequestError(errors));
            }

            List<Partner> existingPartners = await repositoryPartner.GetAllAsync();

            if (DuplicateName(partner, existingPartners))
            {
                return Result.Fail(PartnerErrorResults.DuplicateNameError(request.FullName));
            }

            partner.AssociateTenant(tenantProvider.GetTenantId());

            partner.AssociateUser(user);

            await repositoryPartner.AddAsync(partner);

            await unitOfWork.CommitAsync();

            return Result.Ok(new CreatePartnerResponse(partner.Id));
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

    private static bool DuplicateName(Partner partner, List<Partner> existingPartners)
    {
        return existingPartners
            .Any(entity => string.Equals(
                entity.FullName,
                partner.FullName,
                StringComparison.CurrentCultureIgnoreCase)
            );
    }
}
