using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Rentals.Commands.Delete;

public class DeleteRentalRequestHandler(
    IUnitOfWork unitOfWork,
    IRepositoryRental repositoryRental,
    IDistributedCache cache,
    ILogger<DeleteRentalRequestHandler> logger
) : IRequestHandler<DeleteRentalRequest, Result<DeleteRentalResponse>>
{
    public async Task<Result<DeleteRentalResponse>> Handle(
        DeleteRentalRequest request, CancellationToken cancellationToken)
    {
        Rental? selectedRental = await repositoryRental.GetByIdAsync(request.Id);

        if (selectedRental is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(request.Id));
        }

        if (selectedRental.Status == ERentalStatus.Open)
        {
            return Result.Fail(ErrorResults.BadRequestError("Cannot delete an open rental. Finish or cancel it first."));
        }

        try
        {
            selectedRental.Deactivate();

            logger.LogInformation(
                "Rental {@Id} was deactivated (Soft Delete) to preserve fiscal history.",
                request.Id
            );

            await repositoryRental.UpdateAsync(selectedRental.Id, selectedRental);

            await unitOfWork.CommitAsync();

            await cache.SetStringAsync("rentals:master-version", Guid.NewGuid().ToString(), cancellationToken);
            await cache.SetStringAsync("vehicles:master-version", Guid.NewGuid().ToString(), cancellationToken);

            return Result.Ok(new DeleteRentalResponse());
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