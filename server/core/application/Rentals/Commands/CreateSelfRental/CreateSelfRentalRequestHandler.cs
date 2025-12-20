using AutoMapper;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using LocadoraDeAutomoveis.Application.Rentals.Commands.Create;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.BillingPlans;
using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Coupons;
using LocadoraDeAutomoveis.Domain.Drivers;
using LocadoraDeAutomoveis.Domain.RentalExtras;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Domain.Vehicles;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Rentals.Commands.CreateSelfRental;

public class CreateSelfRentalRequestHandler(
    UserManager<User> userManager,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IRepositoryRental repositoryRental,
    IRepositoryClient repositoryClient,
    IRepositoryDriver repositoryDriver,
    IRepositoryVehicle repositoryVehicle,
    IRepositoryCoupon repositoryCoupon,
    IRepositoryBillingPlan repositoryBillingPlan,
    IRepositoryRentalExtra repositoryRentalExtra,
    IUserContext userContext,
    IRentalEmailService emailService,
    IValidator<Rental> validator,
    ILogger<CreateSelfRentalRequestHandler> logger
) : IRequestHandler<CreateSelfRentalRequest, Result<CreateRentalResponse>>
{
    public async Task<Result<CreateRentalResponse>> Handle(
        CreateSelfRentalRequest request, CancellationToken cancellationToken)
    {
        Guid userId = userContext.GetUserId();

        User? user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(userId));
        }

        Client? client = await repositoryClient.GetByUserIdAsync(userId);

        if (client is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(userId));
        }

        Vehicle? vehicle = await repositoryVehicle.GetByIdDistinctAsync(request.VehicleId);

        if (vehicle is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(request.VehicleId));
        }

        Guid tenantId = vehicle.GetTenantId();

        Driver? driver = await repositoryDriver.GetByTenantAndIdAsync(tenantId, request.DriverId);

        if (driver is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(request.DriverId));
        }

        bool isDriverBusy = await repositoryRental.HasActiveRentalsByDriverDistinctAsync(driver.Id);
        if (isDriverBusy)
        {
            return Result.Fail(ErrorResults.ConflictError("The selected driver currently has an active rental."));
        }

        bool isVehicleRented = await repositoryRental.HasActiveRentalsByVehicleDistinctAsync(vehicle.Id);
        if (isVehicleRented)
        {
            return Result.Fail(ErrorResults.ConflictError("This vehicle is currently rented and not available."));
        }

        BillingPlan? billingPlan = await repositoryBillingPlan.GetByTenantAndGroupAsync(tenantId, vehicle.GroupId);

        if (billingPlan is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(vehicle.GroupId));
        }

        Coupon? coupon = null;
        if (request.CouponId.HasValue && request.CouponId != Guid.Empty)
        {
            coupon = await repositoryCoupon.GetByTenantAndIdAsync(tenantId, request.CouponId.Value);
            if (coupon is null)
            {
                return Result.Fail(ErrorResults.NotFoundError(request.CouponId.Value));
            }

            if (coupon.IsExpired())
            {
                return Result.Fail(ErrorResults.BadRequestError("The coupon is expired."));
            }

            bool alreadyUsed = await repositoryRental.HasCouponUsedByClientDistinctAsync(client.Id, coupon.Id);
            if (alreadyUsed)
            {
                return Result.Fail(ErrorResults.BadRequestError("This client has already used this coupon."));
            }
        }

        List<Guid> requestedExtraIds = request.RentalRentalExtrasIds ?? [];
        List<RentalExtra> extras = [];

        if (requestedExtraIds.Count > 0)
        {
            extras = await repositoryRentalExtra.GetManyByTenantAndIdsDistinctAsync(tenantId, requestedExtraIds);

            if (extras.Count != requestedExtraIds.Count)
            {
                return Result.Fail(ErrorResults.BadRequestError(
                    "One or more selected extras are invalid, unavailable, or do not belong to the selected company."
                ));
            }
        }

        Rental rental = mapper.Map<Rental>(request);
        rental.AssociateTenant(tenantId);
        rental.AssociateUser(user);
        rental.AssociateClient(client);
        rental.AssociateDriver(driver);
        rental.AssociateVehicle(vehicle);
        rental.AssociateBillingPlan(billingPlan);
        rental.SetBillingPlanType(request.BillingPlanType);
        rental.SetStartKm(vehicle.Kilometers);

        if (request.BillingPlanType.Equals(EBillingPlanType.Controlled)
            && request.EstimatedKilometers.HasValue)
        {
            rental.SetEstimatedKilometers(request.EstimatedKilometers.Value);
        }

        if (coupon is not null)
        {
            rental.AssociateCoupon(coupon);
        }

        rental.AddRangeExtras(extras);

        try
        {
            ValidationResult validationResult = await validator.ValidateAsync(rental, cancellationToken);

            if (!validationResult.IsValid)
            {
                List<string> errors = validationResult.Errors
                    .Select(failure => failure.ErrorMessage)
                    .ToList();

                return Result.Fail(ErrorResults.BadRequestError(errors));
            }

            decimal basePrice = RentalCalculator.CalculateBasePrice(rental);
            rental.SetBasePrice(basePrice);

            await repositoryRental.AddAsync(rental);
            await unitOfWork.CommitAsync();

            try
            {
                await emailService.ScheduleRentalConfirmation(rental, client);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to schedule rental confirmation email for rental {RentalId}", rental.Id);
            }

            return Result.Ok(new CreateRentalResponse(rental.Id));
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
}