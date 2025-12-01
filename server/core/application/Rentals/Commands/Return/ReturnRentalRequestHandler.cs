using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using LocadoraDeAutomoveis.Application.Configurations;
using LocadoraDeAutomoveis.Application.Rentals.Services;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Configurations;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Rentals.Commands.Return;

public class ReturnRentalRequestHandler(
    UserManager<User> userManager,
    IUnitOfWork unitOfWork,
    IRepositoryRentalReturn repositoryRentalReturn,
    IRepositoryRental repositoryRental,
    IRepositoryConfiguration repositoryConfiguration,
    ITenantProvider tenantProvider,
    IUserContext userContext,
    IValidator<RentalReturn> validator,
    ILogger<ReturnRentalRequestHandler> logger
) : IRequestHandler<ReturnRentalRequest, Result<ReturnRentalResponse>>
{
    public async Task<Result<ReturnRentalResponse>> Handle(
        ReturnRentalRequest request, CancellationToken cancellationToken)
    {
        User? user = await userManager.FindByIdAsync(userContext.GetUserId().ToString());

        if (user is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(userContext.GetUserId()));
        }

        Guid tenantId = tenantProvider.GetTenantId();
        Configuration? configuration = await repositoryConfiguration.GetByTenantId(tenantId);

        if (configuration is null)
        {
            return Result.Fail(ConfigurationErrorResults.NotFoundForTenant(tenantId));
        }

        Rental? selectedRental = await repositoryRental.GetByIdAsync(request.Id);

        if (selectedRental is null)
        {
            return Result.Fail(ErrorResults.NotFoundError(request.Id));
        }

        DateTimeOffset returnDate = DateTimeOffset.UtcNow;

        int daysUsed = (int)Math.Ceiling((returnDate.Date - selectedRental.StartDate.Date).TotalDays);
        if (daysUsed <= 0)
        {
            daysUsed = 1;
        }

        decimal kmDriven = request.EndKm - selectedRental.StartKm;

        if (kmDriven < 0)
        {
            return Result.Fail(ErrorResults.BadRequestError("The final odometer reading cannot be less than the initial reading."));
        }

        RentalReturn rentalReturn = new(
            returnDate,
            request.EndKm,
            kmDriven
        );
        rentalReturn.SetFuelLevel(request.FuelLevel);
        rentalReturn.AssociateRental(selectedRental);

        try
        {
            ValidationResult validationResult = await validator.ValidateAsync(rentalReturn, cancellationToken);

            if (!validationResult.IsValid)
            {
                List<string> errors = validationResult.Errors
                    .Select(failure => failure.ErrorMessage)
                    .ToList();

                return Result.Fail(ErrorResults.BadRequestError(errors));
            }

            decimal servicesCost = RentalCalculator.CalculateServicesCost(selectedRental.RateServices, daysUsed);
            rentalReturn.SetServicesTotal(servicesCost);

            decimal planCost = RentalCalculator.CalculateRentalPlansCost(
                selectedRental,
                daysUsed,
                kmDriven,
                selectedRental.EstimatedKilometers
            );

            decimal fuelPenalty = RentalCalculator.CalculateFuelPenalty(
                rentalReturn.FuelLevelAtReturn,
                selectedRental.Vehicle,
                configuration
            );

            rentalReturn.SetFuelPenalty(fuelPenalty);

            bool isLateReturn = returnDate.Date > selectedRental.ExpectedReturnDate.Date;
            decimal delayPenalty = 0;

            if (isLateReturn)
            {
                delayPenalty = planCost * 0.10m;
            }

            decimal totalPenalties = delayPenalty + rentalReturn.FuelPenalty;

            rentalReturn.SetPenaltyTotal(totalPenalties);

            decimal finalPrice = planCost + servicesCost + totalPenalties;

            selectedRental.SetStatus(ERentalStatus.Completed);
            selectedRental.ReturnDate = returnDate;
            selectedRental.SetFinalPrice(finalPrice);

            rentalReturn.SetFinalPrice(finalPrice);

            rentalReturn.AssociateTenant(tenantId);
            rentalReturn.AssociateUser(user);

            await repositoryRentalReturn.AddAsync(rentalReturn);

            await unitOfWork.CommitAsync();

            return Result.Ok(new ReturnRentalResponse(rentalReturn.Id));
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync();

            logger.LogError(
                ex,
                "An error occurred during request. \n{@Request}.", request
            );

            return Result.Fail(ErrorResults.InternalServerError(ex));
        }
    }
}
