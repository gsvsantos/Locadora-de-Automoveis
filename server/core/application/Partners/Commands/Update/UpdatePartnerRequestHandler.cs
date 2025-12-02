using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Partners;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Partners.Commands.Update;

public class UpdatePartnerRequestHandler(
    IUnitOfWork unitOfWork,
    IRepositoryPartner repositoryPartner,
    IValidator<Partner> validator,
    ILogger<UpdatePartnerRequestHandler> logger
) : IRequestHandler<UpdatePartnerRequest, Result<UpdatePartnerResponse>>
{
    public async Task<Result<UpdatePartnerResponse>> Handle(
        UpdatePartnerRequest request, CancellationToken cancellationToken)
    {
        Partner? selectedPartner = await repositoryPartner.GetByIdAsync(request.Id);

        if (selectedPartner is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(request.Id));
        }

        Partner updatedPartner = new(
            request.FullName
        )
        { Id = selectedPartner.Id };

        try
        {
            ValidationResult validationResult = await validator.ValidateAsync(updatedPartner, cancellationToken);

            if (!validationResult.IsValid)
            {
                List<string> errors = validationResult.Errors
                    .Select(failure => failure.ErrorMessage)
                    .ToList();

                return Result.Fail(ErrorResults.BadRequestError(errors));
            }

            List<Partner> existingPartners = await repositoryPartner.GetAllAsync();

            if (DuplicateName(updatedPartner, existingPartners))
            {
                return Result.Fail(PartnerErrorResults.DuplicateNameError(request.FullName));
            }

            await repositoryPartner.UpdateAsync(selectedPartner.Id, updatedPartner);

            await unitOfWork.CommitAsync();

            return Result.Ok(new UpdatePartnerResponse(selectedPartner.Id));
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

    private static bool DuplicateName(Partner partner, List<Partner> existingPartners)
    {
        return existingPartners
            .Any(entity =>
            entity.Id != partner.id &&
            string.Equals(
                entity.FullName,
                partner.FullName,
                StringComparison.CurrentCultureIgnoreCase)
            );
    }
}
