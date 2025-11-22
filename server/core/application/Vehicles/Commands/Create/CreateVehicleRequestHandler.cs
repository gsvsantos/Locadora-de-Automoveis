using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Domain.Vehicles;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Vehicles.Commands.Create;

public class CreateVehicleRequestHandler(
    UserManager<User> userManager,
    IUnitOfWork unitOfWork,
    IRepositoryVehicle repositoryVehicle,
    IRepositoryGroup repositoryGroup,
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

        Vehicle vehicle = new(
            request.LicensePlate,
            request.Brand,
            request.Color,
            request.Model,
            request.FuelType,
            request.CapacityInLiters,
            request.Year,
            request.PhotoPath ?? string.Empty
        );

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

            vehicle.AssociateTenant(tenantProvider.GetTenantId());

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

