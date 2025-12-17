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
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Rentals.Commands.Create;

public class CreateRentalRequestHandler(
    UserManager<User> userManager,
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
    ITenantProvider tenantProvider,
    IUserContext userContext,
    IRentalEmailService emailService,
    IValidator<Rental> validator,
    ILogger<CreateRentalRequestHandler> logger
) : IRequestHandler<CreateRentalRequest, Result<CreateRentalResponse>>
{
    public async Task<Result<CreateRentalResponse>> Handle(
        CreateRentalRequest request, CancellationToken cancellationToken)
    {
        User? user = await userManager.FindByIdAsync(userContext.GetUserId().ToString());

        if (user is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(userContext.GetUserId()));
        }

        Employee? employee = null;
        if (request.EmployeeId.HasValue && request.EmployeeId != Guid.Empty)
        {
            employee = await repositoryEmployee.GetByIdAsync(request.EmployeeId.Value);

            if (employee is null)
            {
                return Result.Fail(ErrorResults.NotFoundError(request.EmployeeId.Value));
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

        BillingPlan? BillingPlan = await repositoryBillingPlan.GetByGroupId(vehicle.GroupId);

        if (BillingPlan is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(vehicle.GroupId));
        }

        Coupon? coupon = null;
        if (request.CouponId.HasValue && request.CouponId != Guid.Empty)
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

        Rental rental = mapper.Map<Rental>(request);

        if (employee is not null)
        {
            rental.AssociateEmployee(employee);
        }

        if (coupon is not null)
        {
            rental.AssociateCoupon(coupon);
        }

        rental.AssociateClient(client);
        rental.AssociateDriver(driver);
        rental.AssociateVehicle(vehicle);
        rental.AssociateBillingPlan(BillingPlan);
        rental.SetBillingPlanType(request.BillingPlanType);

        if (request.BillingPlanType.Equals(EBillingPlanType.Controlled)
            && request.EstimatedKilometers.HasValue)
        {
            rental.SetEstimatedKilometers(request.EstimatedKilometers.Value);
        }

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

            List<RentalExtra> RentalExtras = await repositoryRentalExtra.GetManyByIds(request.RentalRentalExtrasIds);

            if (RentalExtras.Count >= 1)
            {
                rental.AddRangeExtras(RentalExtras);
            }

            decimal basePrice = RentalCalculator.CalculateBasePrice(rental);

            rental.SetBasePrice(basePrice);

            rental.AssociateTenant(tenantProvider.GetTenantId());
            rental.AssociateUser(user);

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
