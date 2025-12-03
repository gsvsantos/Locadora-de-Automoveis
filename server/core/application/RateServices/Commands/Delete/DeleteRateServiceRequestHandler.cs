using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.RateServices;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.RateServices.Commands.Delete;

public class DeleteRateServiceRequestHandler(
    IUnitOfWork unitOfWork,
    IRepositoryRateService repositoryRateService,
    IRepositoryRental repositoryRental,
    ILogger<DeleteRateServiceRequestHandler> logger
) : IRequestHandler<DeleteRateServiceRequest, Result<DeleteRateServiceResponse>>
{
    public async Task<Result<DeleteRateServiceResponse>> Handle(
        DeleteRateServiceRequest request, CancellationToken cancellationToken)
    {
        RateService? selectedRateService = await repositoryRateService.GetByIdAsync(request.Id);

        if (selectedRateService is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(request.Id));
        }

        bool hasActiveRentals = await repositoryRental.HasActiveRentalsByRateService(request.Id);
        if (hasActiveRentals)
        {
            return Result.Fail(ErrorResults.BadRequestError("Cannot remove a service associated with active rentals."));
        }

        try
        {
            bool hasHistory = await repositoryRental.HasRentalHistoryByRateService(request.Id);

            if (hasHistory)
            {
                selectedRateService.Deactivate();

                await repositoryRateService.UpdateAsync(selectedRateService.Id, selectedRateService);

                logger.LogInformation("Service '{@Name}' ({@Id}) was deactivated.",
                    selectedRateService.Name,
                    request.Id
                );
            }
            else
            {
                await repositoryRateService.DeleteAsync(request.Id);
                logger.LogInformation("Service '{@Name}' ({@Id}) was permanently deleted.",
                    selectedRateService.Name,
                    request.Id
                );
            }

            await unitOfWork.CommitAsync();

            return Result.Ok(new DeleteRateServiceResponse());
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
