using AutoMapper;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Domain.Vehicles;
using LocadoraDeVeiculos.Infrastructure.S3;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Vehicles.Commands.Create;

public class CreateVehicleRequestHandler(
    UserManager<User> userManager,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IRepositoryVehicle repositoryVehicle,
    IRepositoryGroup repositoryGroup,
    R2FileStorageService fileStorageService,
    ITenantProvider tenantProvider,
    IUserContext userContext,
    IValidator<Vehicle> validator,
    ILogger<CreateVehicleRequestHandler> logger
) : IRequestHandler<CreateVehicleRequest, Result<CreateVehicleResponse>>
{
    public async Task<Result<CreateVehicleResponse>> Handle(
        CreateVehicleRequest request, CancellationToken cancellationToken)
    {
        User? user = await userManager.FindByIdAsync(userContext.GetUserId().ToString());

        if (user is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(userContext.GetUserId()));
        }

        Group? selectedGroup = await repositoryGroup.GetByIdAsync(request.GroupId);

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

        Vehicle vehicle = mapper.Map<Vehicle>((request, imageKey));
        vehicle.SetFuelType(request.FuelType);

        try
        {
            ValidationResult validationResult = await validator.ValidateAsync(vehicle, cancellationToken);

            if (!validationResult.IsValid)
            {
                List<string> errors = validationResult.Errors
                    .Select(failure => failure.ErrorMessage)
                    .ToList();

                return Result.Fail(ErrorResults.BadRequestError(errors));
            }

            List<Vehicle> existingVehicles = await repositoryVehicle.GetAllAsync();

            if (DuplicatePlate(vehicle, existingVehicles))
            {
                return Result.Fail(VehicleErrorResults.DuplicateLicensePlateError(request.LicensePlate));
            }

            vehicle.AssociateTenant(tenantId);

            vehicle.AssociateUser(user);

            vehicle.AssociateGroup(selectedGroup);

            await repositoryVehicle.AddAsync(vehicle);

            await unitOfWork.CommitAsync();

            return Result.Ok(new CreateVehicleResponse(vehicle.Id));
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync();

            logger.LogError(
                ex,
                "An error occurred during registration. \n{@Request}.", request
            );

            return Result.Fail(ErrorResults.InternalServerError(ex));
        }
    }

    private static bool DuplicatePlate(Vehicle vehicle, List<Vehicle> existingVehicles)
    {
        return existingVehicles
            .Any(entity => string.Equals(
                entity.LicensePlate,
                vehicle.LicensePlate,
                StringComparison.CurrentCultureIgnoreCase)
            );
    }
}

