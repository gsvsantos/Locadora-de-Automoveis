using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Domain.Vehicles;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Vehicles.Commands.Update;

public class UpdateVehicleRequestHandler(
    IUnitOfWork unitOfWork,
    IRepositoryVehicle repositoryVehicle,
    IRepositoryGroup repositoryGroup,
    IValidator<Vehicle> validator,
    ILogger<UpdateVehicleRequestHandler> logger
) : IRequestHandler<UpdateVehicleRequest, Result<UpdateVehicleResponse>>
{
    public async Task<Result<UpdateVehicleResponse>> Handle(
        UpdateVehicleRequest request, CancellationToken cancellationToken)
    {
        Vehicle? selectedVehicle = await repositoryVehicle.GetByIdAsync(request.Id);

        if (selectedVehicle is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(request.Id));
        }

        Group? selectedGroup = (selectedVehicle.GroupId != request.GroupId)
                ? await repositoryGroup.GetByIdAsync(request.GroupId)
                : selectedVehicle.Group;

        if (selectedGroup is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(request.GroupId));
        }

        Vehicle updatedVehicle = new(
            request.LicensePlate,
            request.Brand,
            request.Color,
            request.Model,
            request.FuelType,
            request.CapacityInLiters,
            request.Year,
            request.PhotoPath ?? string.Empty
        )
        { Id = selectedVehicle.Id };

        try
        {
            ValidationResult validationResult = await validator.ValidateAsync(updatedVehicle, cancellationToken);

            if (!validationResult.IsValid)
            {
                List<string> errors = validationResult.Errors
                    .Select(failure => failure.ErrorMessage)
                    .ToList();

                return Result.Fail(ErrorResults.BadRequestError(errors));
            }

            selectedVehicle.AssociateGroup(selectedGroup);

            List<Vehicle> existingVehicles = await repositoryVehicle.GetAllAsync();

            if (DuplicatePlate(updatedVehicle, existingVehicles))
            {
                return Result.Fail(VehicleErrorResults.DuplicateLicensePlateError(request.LicensePlate));
            }

            await repositoryVehicle.UpdateAsync(selectedVehicle.Id, updatedVehicle);

            await unitOfWork.CommitAsync();

            return Result.Ok(new UpdateVehicleResponse(selectedVehicle.Id));
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

    private static bool DuplicatePlate(Vehicle vehicle, List<Vehicle> existingVehicles)
    {
        return existingVehicles
            .Any(entity =>
            entity.Id != vehicle.Id &&
            string.Equals(
                entity.LicensePlate,
                vehicle.LicensePlate,
                StringComparison.CurrentCultureIgnoreCase)
            );
    }
}
