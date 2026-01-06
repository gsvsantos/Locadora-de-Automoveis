using AutoMapper;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.RentalExtras;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.RentalExtras.Commands.Update;

public class UpdateRentalExtraRequestHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IRepositoryRentalExtra repositoryRentalExtra,
    IRepositoryRental repositoryRental,
    IDistributedCache cache,
    IValidator<RentalExtra> validator,
    ILogger<UpdateRentalExtraRequestHandler> logger

) : IRequestHandler<UpdateRentalExtraRequest, Result<UpdateRentalExtraResponse>>
{
    public async Task<Result<UpdateRentalExtraResponse>> Handle(
        UpdateRentalExtraRequest request, CancellationToken cancellationToken)
    {
        RentalExtra? selectedRentalExtra = await repositoryRentalExtra.GetByIdAsync(request.Id);

        if (selectedRentalExtra is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(request.Id));
        }

        bool hasActiveRentals = await repositoryRental.HasActiveRentalsByRentalExtra(request.Id);
        if (hasActiveRentals)
        {
            return Result.Fail(ErrorResults.BadRequestError("Cannot edit a extra currently in use by active rentals."));
        }

        RentalExtra updatedRentalExtra = mapper.Map<RentalExtra>(request);
        updatedRentalExtra.DefineType(request.Type);

        try
        {
            ValidationResult validationResult = await validator.ValidateAsync(updatedRentalExtra, cancellationToken);

            if (!validationResult.IsValid)
            {
                List<string> errors = validationResult.Errors
                    .Select(failure => failure.ErrorMessage)
                    .ToList();

                return Result.Fail(ErrorResults.BadRequestError(errors));
            }

            List<RentalExtra> existingRentalExtras = await repositoryRentalExtra.GetAllAsync();

            if (DuplicatedName(updatedRentalExtra, existingRentalExtras))
            {
                return Result.Fail(RentalExtraErrorResults.DuplicateNameError(request.Name));
            }

            if (request.IsDaily)
            {
                updatedRentalExtra.MarkAsFixed();
            }
            else
            {
                updatedRentalExtra.MarkAsDaily();
            }

            await repositoryRentalExtra.UpdateAsync(selectedRentalExtra.Id, updatedRentalExtra);

            await unitOfWork.CommitAsync();

            await cache.SetStringAsync("extras:master-version", Guid.NewGuid().ToString(), cancellationToken);

            return Result.Ok(new UpdateRentalExtraResponse(selectedRentalExtra.Id));

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

    private static bool DuplicatedName(RentalExtra RentalExtra, List<RentalExtra> existingRentalExtras)
    {
        return existingRentalExtras
            .Any(entity =>
            entity.Id != RentalExtra.Id &&
            string.Equals(
                entity.Name,
                RentalExtra.Name,
                StringComparison.CurrentCultureIgnoreCase)
            );
    }
}
