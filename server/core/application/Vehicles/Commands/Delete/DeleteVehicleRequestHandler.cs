using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Domain.Vehicles;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Vehicles.Commands.Delete;

public class DeleteVehicleRequestHandler(
    IUnitOfWork unitOfWork,
    IRepositoryVehicle repositoryVehicle,
    IRepositoryRental repositoryRental,
    ILogger<DeleteVehicleRequestHandler> logger
) : IRequestHandler<DeleteVehicleRequest, Result<DeleteVehicleResponse>>
{
    public async Task<Result<DeleteVehicleResponse>> Handle(
        DeleteVehicleRequest request, CancellationToken cancellationToken)
    {
        Vehicle? selectedVehicle = await repositoryVehicle.GetByIdAsync(request.Id);

        if (selectedVehicle is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(request.Id));
        }

        bool hasActiveRentals = await repositoryRental.HasActiveRentalsByVehicle(request.Id);
        if (hasActiveRentals)
        {
            return Result.Fail(ErrorResults.BadRequestError("Cannot remove a vehicle associated with active rentals."));
        }

        try
        {
            bool hasHistory = await repositoryRental.HasRentalHistoryByVehicle(request.Id);

            if (hasHistory)
            {
                selectedVehicle.Deactivate();

                await repositoryVehicle.UpdateAsync(selectedVehicle.Id, selectedVehicle);

                logger.LogInformation(
                    "Vehicle '{@LicensePlate}' ({@Id}) was deactivated to preserve rental history.",
                    selectedVehicle.LicensePlate,
                    request.Id
                );
            }
            else
            {
                await repositoryVehicle.DeleteAsync(request.Id);

                logger.LogInformation(
                    "Vehicle '{@LicensePlate}' ({@Id}) was permanently deleted.",
                    selectedVehicle.LicensePlate,
                    request.Id
                );
            }

            await unitOfWork.CommitAsync();

            return Result.Ok(new DeleteVehicleResponse());
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
