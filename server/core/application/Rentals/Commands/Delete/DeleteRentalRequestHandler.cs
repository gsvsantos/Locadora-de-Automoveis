using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Rentals.Commands.Delete;

public class DeleteRentalRequestHandler(
    IUnitOfWork unitOfWork,
    IRepositoryRental repositoryRental,
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

        try
        {
            await repositoryRental.DeleteAsync(request.Id);

            await unitOfWork.CommitAsync();

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