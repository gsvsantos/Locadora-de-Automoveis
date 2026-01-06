using AutoMapper;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.BillingPlans;
using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Coupons;
using LocadoraDeAutomoveis.Domain.Drivers;
using LocadoraDeAutomoveis.Domain.Employees;
using LocadoraDeAutomoveis.Domain.RentalExtras;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Domain.Vehicles;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Rentals.Commands.Update;

public class UpdateRentalRequestHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IRepositoryRental repositoryRental,
    IRepositoryEmployee repositoryEmployee,
    IRepositoryClient repositoryClient,
    IRepositoryDriver repositoryDriver,
    IRepositoryVehicle repositoryVehicle,
    IRepositoryCoupon repositoryCoupon,
    IRepositoryBillingPlan repositoryBillingPlan,
    IRepositoryRentalExtra repositoryRentalExtra,
    IDistributedCache cache,
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

        if (selectedRental.Status == ERentalStatus.Completed)
        {
            return Result.Fail(ErrorResults.BadRequestError("Cannot edit a completed rental."));
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

        if (driver.LicenseValidity < DateTimeOffset.UtcNow)
        {
            return Result.Fail(ErrorResults.BadRequestError("The driver's license is expired."));
        }

        Vehicle? vehicle = await repositoryVehicle.GetByIdAsync(request.VehicleId);

        if (vehicle is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(request.VehicleId));
        }

        if (selectedRental.VehicleId != request.VehicleId)
        {
            bool isVehicleRented = await repositoryRental.HasActiveRentalsByVehicle(request.VehicleId);
            if (isVehicleRented)
            {
                return Result.Fail(ErrorResults.BadRequestError("The new selected vehicle is currently rented."));
            }
        }

        BillingPlan? BillingPlan = await repositoryBillingPlan.GetByGroupId(vehicle.GroupId);

        if (BillingPlan is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(vehicle.GroupId));
        }

        Coupon? coupon = null;
        if (request.CouponId.HasValue && request.CouponId != Guid.Empty)
        {
            if (selectedRental.CouponId != request.CouponId)
            {
                coupon = await repositoryCoupon.GetByIdAsync(request.CouponId.Value);
                if (coupon is null)
                {
                    return Result.Fail(ErrorResults.NotFoundError(request.CouponId.Value));
                }

                if (coupon.IsExpired())
                {
                    return Result.Fail(ErrorResults.BadRequestError("The coupon is expired."));
                }

                bool alreadyUsed = await repositoryRental.HasClientUsedCoupon(client.Id, coupon.Id);
                if (alreadyUsed)
                {
                    return Result.Fail(ErrorResults.BadRequestError("This client has already used this coupon."));
                }
            }
            else
            {
                coupon = selectedRental.Coupon;
            }
        }
        else
        {
            coupon = selectedRental.Coupon;
        }

        Rental updatedRental = mapper.Map<Rental>(request);

        if (employee is not null)
        {
            updatedRental.AssociateEmployee(employee);
        }

        if (coupon is not null)
        {
            updatedRental.AssociateCoupon(coupon);
        }

        updatedRental.AssociateClient(client);
        updatedRental.AssociateDriver(driver);
        updatedRental.AssociateVehicle(vehicle);
        updatedRental.AssociateBillingPlan(BillingPlan);
        updatedRental.SetBillingPlanType(request.BillingPlanType);

        if (request.BillingPlanType.Equals(EBillingPlanType.Controlled)
            && request.EstimatedKilometers.HasValue)
        {
            updatedRental.SetEstimatedKilometers(request.EstimatedKilometers.Value);
        }

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

            List<RentalExtra> rentalExtras = await repositoryRentalExtra.GetManyByIds(request.RentalRentalExtrasIds);

            if (rentalExtras.Count >= 1)
            {
                updatedRental.AddRangeExtras(rentalExtras);
            }

            decimal basePrice = RentalCalculator.CalculateBasePrice(updatedRental);

            updatedRental.SetBasePrice(basePrice);

            await repositoryRental.UpdateAsync(selectedRental.Id, updatedRental);

            await unitOfWork.CommitAsync();

            await cache.SetStringAsync("rentals:master-version", Guid.NewGuid().ToString(), cancellationToken);
            await cache.SetStringAsync("vehicles:master-version", Guid.NewGuid().ToString(), cancellationToken);

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
