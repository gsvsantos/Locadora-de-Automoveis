using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.RentalExtras;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.RentalExtras.Commands.Delete;

public class DeleteRentalExtraRequestHandler(
    IUnitOfWork unitOfWork,
    IRepositoryRentalExtra repositoryRentalExtra,
    IRepositoryRental repositoryRental,
    IDistributedCache cache,
    ILogger<DeleteRentalExtraRequestHandler> logger
) : IRequestHandler<DeleteRentalExtraRequest, Result<DeleteRentalExtraResponse>>
{
    public async Task<Result<DeleteRentalExtraResponse>> Handle(
        DeleteRentalExtraRequest request, CancellationToken cancellationToken)
    {
        RentalExtra? selectedRentalExtra = await repositoryRentalExtra.GetByIdAsync(request.Id);

        if (selectedRentalExtra is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(request.Id));
        }

        bool hasActiveRentals = await repositoryRental.HasActiveRentalsByRentalExtra(request.Id);
        if (hasActiveRentals)
        {
            return Result.Fail(ErrorResults.BadRequestError("Cannot remove a extra associated with active rentals."));
        }

        try
        {
            bool hasHistory = await repositoryRental.HasRentalHistoryByRentalExtra(request.Id);

            if (hasHistory)
            {
                selectedRentalExtra.Deactivate();

                await repositoryRentalExtra.UpdateAsync(selectedRentalExtra.Id, selectedRentalExtra);

                logger.LogInformation("Extra '{@Name}' ({@Id}) was deactivated.",
                    selectedRentalExtra.Name,
                    request.Id
                );
            }
            else
            {
                await repositoryRentalExtra.DeleteAsync(request.Id);
                logger.LogInformation("Extra '{@Name}' ({@Id}) was permanently deleted.",
                    selectedRentalExtra.Name,
                    request.Id
                );
            }

            await unitOfWork.CommitAsync();

            await cache.SetStringAsync("extras:master-version", Guid.NewGuid().ToString(), cancellationToken);

            return Result.Ok(new DeleteRentalExtraResponse());
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
