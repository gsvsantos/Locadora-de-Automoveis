using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Domain.Vehicles;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Vehicles.Commands.Delete;

public class DeleteVehicleRequestHandler(
    IUnitOfWork unitOfWork,
    IRepositoryVehicle repositoryVehicle,
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

        try
        {
            await repositoryVehicle.DeleteAsync(request.Id);

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
