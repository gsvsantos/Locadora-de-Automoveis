using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.RateServices;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.RateServices.Commands.Update;

public class UpdateRateServiceRequestHandler(
    IUnitOfWork unitOfWork,
    IRepositoryRateService repositoryRateService,
    IValidator<RateService> validator,
    ILogger<UpdateRateServiceRequestHandler> logger

) : IRequestHandler<UpdateRateServiceRequest, Result<UpdateRateServiceResponse>>
{
    public async Task<Result<UpdateRateServiceResponse>> Handle(
        UpdateRateServiceRequest request, CancellationToken cancellationToken)
    {
        RateService? selectedRateService = await repositoryRateService.GetByIdAsync(request.Id);

        if (selectedRateService is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(request.Id));
        }

        RateService updatedRateService = new(
            request.Name,
            request.Price
        )
        { Id = selectedRateService.Id };

        try
        {
            ValidationResult validationResult = await validator.ValidateAsync(updatedRateService, cancellationToken);

            if (!validationResult.IsValid)
            {
                List<string> errors = validationResult.Errors
                    .Select(failure => failure.ErrorMessage)
                    .ToList();

                return Result.Fail(ErrorResults.BadRequestError(errors));
            }

            List<RateService> existingRateServices = await repositoryRateService.GetAllAsync();

            if (DuplicatedName(updatedRateService, existingRateServices))
            {
                return Result.Fail(RateServiceErrorResults.DuplicateNameError(request.Name));
            }

            if (request.IsFixed)
            {
                updatedRateService.MarkAsFixed();
            }
            else
            {
                updatedRateService.MarkAsDaily();
            }

            await repositoryRateService.UpdateAsync(selectedRateService.Id, updatedRateService);

            await unitOfWork.CommitAsync();

            return Result.Ok(new UpdateRateServiceResponse(selectedRateService.Id));

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

    private static bool DuplicatedName(RateService rateService, List<RateService> existingRateServices)
    {
        return existingRateServices
            .Any(entity =>
            entity.Id != rateService.Id &&
            string.Equals(
                entity.Name,
                rateService.Name,
                StringComparison.CurrentCultureIgnoreCase)
            );
    }
}
