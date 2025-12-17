using AutoMapper;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Domain.Vehicles;
using LocadoraDeVeiculos.Infrastructure.S3;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Vehicles.Commands.Update;

public class UpdateVehicleRequestHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IRepositoryVehicle repositoryVehicle,
    IRepositoryGroup repositoryGroup,
    IRepositoryRental repositoryRental,
    R2FileStorageService fileStorageService,
    ITenantProvider tenantProvider,
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

        bool hasActiveRentals = await repositoryRental.HasActiveRentalsByVehicle(request.Id);
        if (hasActiveRentals)
        {
            return Result.Fail(ErrorResults.BadRequestError("Cannot edit a vehicle that is currently rented."));
        }

        Group? selectedGroup = (selectedVehicle.GroupId != request.GroupId)
                ? await repositoryGroup.GetByIdAsync(request.GroupId)
                : selectedVehicle.Group;

        if (selectedGroup is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(request.GroupId));
        }

        Guid tenantId = tenantProvider.GetTenantId();

        string imageKey = string.Empty;
        if (request.Image is { Length: > 0 })
        {
            string extension = Path.GetExtension(request.Image.FileName);

            if (string.IsNullOrWhiteSpace(extension))
            {
                extension = ".jpg";
            }

            string key = $"vehicles/{tenantId}/{Guid.NewGuid()}{extension}";
            string contentType = request.Image.ContentType;

            await using Stream stream = request.Image.OpenReadStream();

            imageKey = await fileStorageService.UploadAsync(stream, contentType, key, cancellationToken);
        }

        Vehicle updatedVehicle = mapper.Map<Vehicle>((request, imageKey));
        updatedVehicle.SetFuelType(request.FuelType);

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
