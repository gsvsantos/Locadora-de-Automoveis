using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Drivers;
using LocadoraDeAutomoveis.Domain.Employees;
using LocadoraDeAutomoveis.Domain.PricingPlans;
using LocadoraDeAutomoveis.Domain.RateServices;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Domain.Vehicles;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Rentals.Commands.Update;

public class UpdateRentalRequestHandler(
    IUnitOfWork unitOfWork,
    IRepositoryRental repositoryRental,
    IRepositoryEmployee repositoryEmployee,
    IRepositoryClient repositoryClient,
    IRepositoryDriver repositoryDriver,
    IRepositoryVehicle repositoryVehicle,
    IRepositoryPricingPlan repositoryPricingPlan,
    IRepositoryRateService repositoryRateService,
    IUserContext userContext,
    IValidator<Rental> validator,
    ILogger<UpdateRentalRequestHandler> logger
) : IRequestHandler<UpdateRentalRequest, Result<UpdateRentalResponse>>
{
    public async Task<Result<UpdateRentalResponse>> Handle(
        UpdateRentalRequest request, CancellationToken cancellationToken)
    {
        Rental? selectedRental = await repositoryRental.GetByIdAsync(request.Id);

        if (selectedRental is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(request.Id));
        }

        Employee? employee = null;
        if (request.EmployeeId.HasValue && request.EmployeeId != Guid.Empty)
        {
            employee = await repositoryEmployee.GetByIdAsync(request.EmployeeId.Value);

            if (employee is null)
            {
                return Result.Fail(ErrorResults.NotFoundError(userContext.GetUserId()));
            }
        }

        Client? client = await repositoryClient.GetByIdAsync(request.ClientId);

        if (client is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(request.ClientId));
        }

        Driver? driver = await repositoryDriver.GetByIdAsync(request.DriverId);

        if (driver is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(request.DriverId));
        }

        Vehicle? vehicle = await repositoryVehicle.GetByIdAsync(request.VehicleId);

        if (vehicle is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(request.VehicleId));
        }

        PricingPlan? pricingPlan = await repositoryPricingPlan.GetByGroupId(vehicle.GroupId);

        if (pricingPlan is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(vehicle.GroupId));
        }

        Rental updatedRental = new(
            request.StartDate,
            request.ExpectedReturnDate,
            request.StartKm
        )
        { Id = selectedRental.Id };

        if (employee is not null)
        {
            updatedRental.AssociateEmployee(employee);
        }

        updatedRental.AssociateClient(client);
        updatedRental.AssociateDriver(driver);
        updatedRental.AssociateVehicle(vehicle);
        updatedRental.AssociatePricingPlan(pricingPlan);

        try
        {
            ValidationResult validationResult = await validator.ValidateAsync(updatedRental, cancellationToken);

            if (!validationResult.IsValid)
            {
                List<string> errors = validationResult.Errors
                    .Select(failure => failure.ErrorMessage)
                    .ToList();

                return Result.Fail(ErrorResults.BadRequestError(errors));
            }

            List<RateService> rateServices = await repositoryRateService.GetManyByIds(request.RentalRateServicesIds);

            if (rateServices.Count >= 1)
            {
                updatedRental.AddRangeRateServices(rateServices);
            }

            updatedRental.CalculateBasePrice();
            updatedRental.SetStatus(ERentalStatus.Open);

            await repositoryRental.UpdateAsync(selectedRental.Id, updatedRental);

            await unitOfWork.CommitAsync();

            return Result.Ok(new UpdateRentalResponse(selectedRental.Id));
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
